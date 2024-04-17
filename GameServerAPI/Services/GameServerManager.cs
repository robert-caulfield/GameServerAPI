using AutoMapper;
using Microsoft.Extensions.Options;
using GameServerAPI.BackgroundServices;
using GameServerAPI.Configuration;
using GameServerAPI.Data;
using GameServerAPI.Models;
using GameServerAPI.Models.DTO.GameServer;
using GameServerAPI.Models.Token;
using GameServerAPI.Repository.IRepository;
using GameServerAPI.Services.Token;
using GameServerAPI.Services.Token.IServices;
using System.Net;

namespace GameServerAPI.Services
{
    /// <summary>
    /// Service responsible for the management of game servers
    /// utilizing the data stored in <see cref="GameServerMemoryStore"/>
    /// and <see cref="ApplicationDBContext"/>
    /// </summary>
    public class GameServerManager
    {
        private readonly GameServerManagerSettings _settings;
        private readonly GameServerMemoryStore _memoryStore;

        private readonly IMapper _mapper;
        private readonly IGameServerRepository _gameServerRepository;


        /// <summary>
        /// Initializes instance of GameServerManager.
        /// </summary>
        /// <param name="settings">Instance of IOptions for accessing Game Server Manager settings, such as heartbeat timeout.</param>
        /// <param name="memoryStore">Reference to game server memory store object used for accessing in-memory storage.</param>
        /// <param name="gameServerRepo">Instance of IGameServerRepository used for manipulating and accessing game server database objects.</param>
        /// <param name="mapper">Instance of IMapper for mapping of different game server objects.</param>
        public GameServerManager(IOptions<GameServerManagerSettings> settings, GameServerMemoryStore memoryStore, IGameServerRepository gameServerRepo, IMapper mapper)
        {
            _settings = settings.Value;
            _memoryStore = memoryStore;
            _gameServerRepository = gameServerRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Updates a game servers player count
        /// </summary>
        /// <param name="serverId">The id of the game server</param>
        /// <param name="newValue">The new value of playercount</param>
        public void UpdatePlayerCount(string serverId, int newValue)
        {
            _memoryStore.UpdateServerCache(serverId, cache => cache.PlayerCount = newValue);
        }

        /// <summary>
        /// Sets a game server last heartbeat to the current time
        /// </summary>
        /// <param name="serverId">The id of the game server</param>
        public void UpdateHeartbeat(string serverId)
        {
            _memoryStore.UpdateServerCache(serverId, cache => cache.LastHeartbeat = DateTime.UtcNow);
        }

        /// <summary>
        /// Removes all game servers that have a last heartbeat
        /// that exceeds <see cref="GameServerManagerSettings.HeartbeatTimeout"/>
        /// </summary>
        /// <remarks>
        /// Used by <see cref="GameServerCleanupService"/> background service
        /// </remarks>
        public async Task RemoveInactiveServers()
        {
            // Get current time to compare to last heartbeat
            DateTime currentTime = DateTime.UtcNow;

            // Get servers that timed out
            List<string> timedOutServers = _memoryStore.ServerCache
                .Where(s => currentTime - s.Value.LastHeartbeat > TimeSpan.FromSeconds(_settings.HeartbeatTimeout))
                .Select(s => s.Key)
                .ToList();


            foreach (string serverId in timedOutServers)
            {
                // Get game server associated with id and remove it
                GameServer? gameServer = await _gameServerRepository.GetAsync(x => x.Id == serverId);
                if (gameServer != null)
                {
                    await RemoveGameServer(gameServer);
                }
                else
                { // Value does not exist in database, just remove in memory
                    _memoryStore.RemoveServerCache(serverId);
                }
            }

        }

        /// <summary>
        /// Adds a game server to database and in memory
        /// </summary>
        /// <param name="gameServer">The game server to be removed</param>
        public async Task<GameServer?> AddGameServer(GameServer gameServer)
        {
            GameServer entity = await _gameServerRepository.CreateAsync(gameServer);
            if (entity != null)
            {
                // Add server cahce
                _memoryStore.UpdateServerCache(entity.Id, cache =>
                {
                    cache.LastHeartbeat = DateTime.UtcNow;
                    cache.PlayerCount = 0;
                });
                return entity;
            }
            return null;
        }

        /// <summary>
        /// Removes a game server from database and in memory store
        /// </summary>
        /// <param name="gameServer">The game server to be removed</param>
        /// <param name="saveChanges">If the changes should be saved to database</param>
        public async Task RemoveGameServer(GameServer gameServer)
        {
            await _gameServerRepository.RemoveAsync(gameServer);
            _memoryStore.RemoveServerCache(gameServer.Id);
        }

        /// <summary>
        /// Gets all game servers and returns them as a list of GameServerDetailsDTOs,
        /// populating the playercount property.
        /// </summary>
        /// <returns>
        /// A list of GameServerDetailsDTOs with populated playercounts. If a playercount is not
        /// found it is set to -1.
        /// </returns>
        public async Task<List<GameServerDetailsDTO>> GetAllGameServerDetails()
        {
            List<GameServer>? gameServers = await _gameServerRepository.GetAllAsync();
            List<GameServerDetailsDTO> result = new List<GameServerDetailsDTO>();
            if (gameServers != null)
            {
                foreach(GameServer gameServer in gameServers)
                {
                    // Map GameServer to GameServerDetail
                    GameServerDetailsDTO gameServerDetails = _mapper.Map<GameServerDetailsDTO>(gameServer);

                    // Populate with properties from cache
                    gameServerDetails = _mapper.Map(_memoryStore.GetServerCache(gameServerDetails.Id), gameServerDetails);

                    // Add to result
                    result.Add(gameServerDetails);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrives a game server by its id if the user is authorized.
        /// Authorization is based on the user's role and ownership of the game server, or if user is an admin
        /// </summary>
        /// <remarks>
        /// Users with the role of 'Server' can only access game servers they own: <paramref name="userId"/> == <see cref="GameServer.CreatorUserId"/>
        /// Users with the role of 'Admin' can access all game servers.
        /// </remarks>
        /// <param name="serverId">The id of the game server to retrieve.</param>
        /// <param name="userId">The user's ID, used to check if they are the owner of the game server.</param>
        /// <param name="userRole">The role of the user, used to determine access level.</param>
        /// <returns>A <see cref="ServiceResult{GameServer}"/> with the result being the desired game server if retrieval was successful.</returns>
        public async Task<ServiceResult<GameServer>> GetGameServerIfAuthorized(string serverId, string userId, string userRole)
        {
            // User id and role null check
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return new ServiceResult<GameServer>()
                {
                    Errors = new List<string>() {"User Id or role is null or empty."},
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            // Server Id null check
            if (string.IsNullOrEmpty(serverId))
            {
                return new ServiceResult<GameServer>()
                {
                    Errors = new List<string>() { "Server Id is null or empty." },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            GameServer? server = await _gameServerRepository.GetAsync(x => x.Id == serverId);
            // Check if game server associated with id exists
            if (server == null)
            {
                return new ServiceResult<GameServer>()
                {
                    Errors = new List<string>() { "Sever ID not associated with any game server" },
                    StatusCode = HttpStatusCode.Forbidden
                };
            }

            // (Check if user has 'Server' role and is owner of game server) or if the user is an admin
            if ((userRole.ToLower().Equals("server") && userId == server.CreatorUserId) || userRole.ToLower().Equals("admin"))
            {
                //Update heartbeat
                UpdateHeartbeat(server.Id);

                // Return service result with the result being the game server
                return new ServiceResult<GameServer>()
                {
                    StatusCode = HttpStatusCode.OK,
                    Result = server
                };
            }
            else
            { // User is not authorized to access game server
                return new ServiceResult<GameServer>()
                {
                    Errors = new List<string>() { "User is unauthorized to modify this game server." },
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
    }
}
