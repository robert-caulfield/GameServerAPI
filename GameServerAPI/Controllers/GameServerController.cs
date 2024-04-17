using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using GameServerAPI.Data;
using GameServerAPI.Models;
using GameServerAPI.Models.API;
using GameServerAPI.Models.DTO.GameServer;
using GameServerAPI.Models.TokenObjects;
using GameServerAPI.Repository.IRepository;
using GameServerAPI.Services;
using GameServerAPI.Services.Token.IServices;
using GameServerAPI.SwaggerExamples;
using GameServerAPI.Utilities;
using Swashbuckle.AspNetCore.Filters;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace GameServerAPI.Controllers
{
    /// <summary>
    /// Controller allowing game servers to register themselves to the API and
    /// manage their data. This includes validating players, handling disconnects,
    /// sending heartbeats, and deregistering themselves from the API.
    /// </summary>
    [Produces("application/json")]
    [Route("api/game-servers")]
    [ApiController]
    public class GameServerController : Controller
    {
        private readonly IMapper _mapper;


        private readonly GameServerMemoryStore _gameServerMemoryStore;
        private ITokenService<PlayerJoinToken> _playerJoinTokenService;

        private readonly IUserRepository _userRepo;

        private readonly GameServerManager _gameServerManager;
        private readonly IGameServerRepository _gameServerRepo;

        
        /// <summary>
        /// Initializes instance of GameServerController.
        /// </summary>
        /// <param name="mapper">An IMapper instance used for object mapping.</param>
        /// <param name="playerTokenService">An ITokenService instance used for validation of player join tokens.</param>
        /// <param name="userRepository">An IUserRepository instance used for accessing user data.</param>
        /// <param name="gameServerManager">A GameServerManager instance used for managing game server data.</param>
        /// <param name="gameServerRepo">A IGameServerRepository instance used for gameserver database management.</param>
        /// <param name="gameServerMemoryStore">Game server memory store.</param>
        public GameServerController(
            IMapper mapper,
            ITokenService<PlayerJoinToken> playerTokenService,
            IUserRepository userRepository,
            GameServerManager gameServerManager,
            IGameServerRepository gameServerRepo,
            GameServerMemoryStore gameServerMemoryStore)
        {
            _mapper = mapper;
            _playerJoinTokenService = playerTokenService;
            _userRepo = userRepository;
            _gameServerManager = gameServerManager;
            _gameServerRepo = gameServerRepo;
            _gameServerMemoryStore = gameServerMemoryStore;
        }

        /// <summary>
        /// Registers a new game server to be stored in the API.
        /// </summary>
        /// <param name="registerDTO">Data required to register a new game server.</param>
        /// <returns>
        /// An ActionResult containing an APIResponse with the APIResponse.Result being the
        /// Id of the new game server. The Id will is used in the route of future requests.
        /// </returns>
        /// <response code="200">Successful. Returns Server Id in APIResponse.Result.</response>
        /// <response code="400">Could not register game server with provided information.</response>
        /// <response code="500">Server error encountered while registering game server.</response>
        /// <response code="401">Request is unauthorized.</response>
        [Authorize(Roles ="Admin, Server")]
        [HttpPost]
        [ProducesResponseType(typeof(Models.API.SwaggerExample.APIResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(ServerErrorExample))]
        public async Task<ActionResult<APIResponse>> RegisterServer([FromBody] GameServerRegisterDTO registerDTO)
        {
            // Ensure valid model state
            if (!ModelState.IsValid)
            {
                return ApiControllerUtilities.HandleInvalidModelState(HttpStatusCode.BadRequest, ModelState);
            }

            // Map the register dto to a game server object
            GameServer gameServer = _mapper.Map<GameServer>(registerDTO);
            
            // Insure that ip and port are unique
            if(await _gameServerRepo.GetAsync(x=> x.IPAddress == gameServer.IPAddress && x.Port == gameServer.Port) != null)
            {
                return ApiControllerUtilities.HandleError(HttpStatusCode.Conflict, "Server with that ip and port already exists!");
            }

            // Get user id
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // User id null check
            if (userId == null)
            {
                return ApiControllerUtilities.HandleError(HttpStatusCode.BadRequest, "Unable to retrieve user id from auth token.");
            }
            // Populate creator id field of game server with user's id
            gameServer.CreatorUserId = userId;

            // Add database and ensure successful creation
            GameServer? entity = await _gameServerManager.AddGameServer(gameServer);
            if (entity == null)
            {
                return ApiControllerUtilities.HandleError(HttpStatusCode.InternalServerError, "Server could not be created.");
            }

            // Return Ok result, and an APIResponse object containing the id of the new game server.
            return ApiControllerUtilities.OkResult(gameServer.Id);
        }

        /// <summary>
        /// Validates a PlayerJoinToken, and returns player information if validation is successful
        /// </summary>
        /// <remarks>
        /// Ensures that:<br/>
        ///     - The token is valid.<br/>
        ///     - The PlayerToken.ServerId matches the requesting server's Id.<br/>
        /// </remarks>
        /// <param name="id">String that represents the id of the game server.</param>
        /// <param name="validatePlayerDTO">A DTO containing the PlayerJoinToken to be validated.</param>
        /// <returns>
        /// An ActionResult containing an APIResponse with the APIResponse.Result
        /// being a PlayerProfileDTO which contains information about the player who was validated.
        /// </returns>
        /// <response code="200">Successful. Returns PlayerProfileDTO in APIResponse.Result.</response>
        /// <response code="400">Provided PlayerJoinToken is invalid.</response>
        /// <response code="401">Request is unauthorized.</response>
        /// <response code="403">Provided Id isnt associated with a game server, or invalid access to game server.</response>
        [Authorize(Roles = "Admin, Server")]
        [HttpPost("{id}/players/validate")]
        [ProducesResponseType(typeof(Models.API.SwaggerExample.APIResponse<PlayerProfileDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status403Forbidden)]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenExample))]
        public async Task<ActionResult<APIResponse>> ValidatePlayer([FromRoute] string id, [FromBody] GameServerValidatePlayerDTO validatePlayerDTO)
        {
            // Ensure valid model state
            if (!ModelState.IsValid)
            {
                return ApiControllerUtilities.HandleInvalidModelState(HttpStatusCode.BadRequest, ModelState);
            }
            // Helper method that retrieves a game server if a user is authorized
            ServiceResult<GameServer> serviceResponse = await _gameServerManager.GetGameServerIfAuthorized(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst(ClaimTypes.Role)?.Value);

            // If token verification was unsucccesful
            if (!serviceResponse.IsSuccess)
            {
                // Return response provided by helper method
                return ApiControllerUtilities.HandleError(serviceResponse.StatusCode, serviceResponse.Errors);
            }

            // Succesful result means that result is not null
            GameServer gameServer = serviceResponse.Result;

            // Validate player token
            PlayerJoinToken? playerJoinToken = _playerJoinTokenService.ValidateToken(validatePlayerDTO.PlayerJoinToken);

            // Check if player token is valid
            if (playerJoinToken == null)
            {
                return ApiControllerUtilities.HandleError(HttpStatusCode.BadRequest, "Error validating token");
            }

            // Check if join token matches validating server
            if (gameServer.Id != playerJoinToken.ServerId)
            {
                return ApiControllerUtilities.HandleError(HttpStatusCode.BadRequest, "The player join token does not match the server.");
            }
            
            // Get user profile 
            var user = await _userRepo.GetAsync(x => x.Id == playerJoinToken.Id);
            if (user == null)
            {
                return ApiControllerUtilities.HandleError(HttpStatusCode.BadRequest, "No user associated with token ID.");
            }

            // Create and return user profile
            PlayerProfileDTO playerProfile = new PlayerProfileDTO()
            {
                Id = playerJoinToken.Id,
                Username = user.UserName
            };

            return ApiControllerUtilities.OkResult(playerProfile);

        }

        /// <summary>
        /// Removes the GameServer from the API.
        /// </summary>
        /// <param name="id">String that represents the id of the game server.</param>
        /// <returns>No content response if the game server was successfully removed.</returns>
        /// <response code="204">Game server successfully removed.</response>
        /// <response code="400">Invalid auth token or game server Id.</response>
        /// <response code="401">Request is unauthorized.</response>
        /// <response code="403">Provided Id isnt associated with a game server, or invalid access to game server.</response>
        [Authorize(Roles = "Admin, Server")]
        [HttpDelete("{id}")]
        
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status403Forbidden)]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenExample))]
        public async Task<ActionResult<APIResponse>> CloseServer([FromRoute] string id)
        {
            // Helper method that retrieves a game server if a user is authorized
            ServiceResult<GameServer> serviceResponse = await _gameServerManager.GetGameServerIfAuthorized(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst(ClaimTypes.Role)?.Value);

            // If token verification was unsucccesful
            if (!serviceResponse.IsSuccess)
            {
                // Return response provided by helper method
                return ApiControllerUtilities.HandleError(serviceResponse.StatusCode, serviceResponse.Errors);
            }

            // Succesful result means that result is not null
            GameServer gameServer = serviceResponse.Result;

            // Remove listed server
            await _gameServerManager.RemoveGameServer(gameServer);

            return NoContent();
        }

        /// <summary>
        /// Validates that a server key is associated with a registered game server
        /// and updates the game server's heartbeat.
        /// </summary>
        /// <param name="id">String that represents the id of the game server.</param>
        /// <returns>
        /// No content response if game server was successfully validated and heartbeat was updated.
        /// </returns>
        /// <response code="204">Heartbeat successfully updated.</response>
        /// <response code="400">Provided ServerToken is null or empty.</response>
        /// <response code="401">Request is unauthorized.</response>
        /// <response code="403">Provided Id isnt associated with a game server, or invalid access to game server.</response>
        [Authorize(Roles = "Admin, Server")]
        [HttpPost("{id}/heartbeat")]
        
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status403Forbidden)]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenExample))]
        public async Task<ActionResult<APIResponse>> Heartbeat([FromRoute] string id)
        {
            // Helper method that retrieves a game server if a user is authorized
            ServiceResult<GameServer> serviceResponse = await _gameServerManager.GetGameServerIfAuthorized(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst(ClaimTypes.Role)?.Value);

            // If token verification was unsucccesful
            if (!serviceResponse.IsSuccess)
            {
                // Return response provided by helper method
                return ApiControllerUtilities.HandleError(serviceResponse.StatusCode, serviceResponse.Errors);
            }

            return NoContent();
        }

        /// <summary>
        /// Updates the cache of a game server using a JSON patch document
        /// </summary>
        /// <param name="id">String that represents the id of the game server.</param>
        /// <returns>
        /// API response indicating the result of the operation.
        /// </returns>
        /// <response code="204">The cache was successfully updated.</response>
        /// <response code="400">Patch document invalid or Serverkey is null or empty.</response>
        /// <response code="401">Request is unauthorized.</response>
        /// <response code="403">Provided Id isnt associated with a game server, or invalid access to game server.</response>
        [Authorize(Roles = "Admin, Server")]
        [HttpPatch("{id}/cache")]
        
        
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status403Forbidden)]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenExample))]
        public async Task<ActionResult<APIResponse>> UpdateCache([FromRoute] string id, [FromBody] JsonPatchDocument<GameServerCache> patchDoc)
        {
            // Helper method that retrieves a game server if a user is authorized
            ServiceResult<GameServer> serviceResponse = await _gameServerManager.GetGameServerIfAuthorized(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst(ClaimTypes.Role)?.Value);

            // If token verification was unsucccesful
            if (!serviceResponse.IsSuccess)
            {
                // Return response provided by helper method
                return ApiControllerUtilities.HandleError(serviceResponse.StatusCode, serviceResponse.Errors);
            }

            // Succesful result means that result is not null
            GameServer gameServer = serviceResponse.Result;

            if (patchDoc == null)
            {
                return ApiControllerUtilities.HandleError(HttpStatusCode.BadRequest, "Patch document is null");
            }

            try
            {
                _gameServerMemoryStore.UpdateServerCache(gameServer.Id, existingCache =>
                {
                    // deep copy
                    var cacheToPatch = _mapper.Map<GameServerCache>(existingCache);

                    // Apply patch
                    patchDoc.ApplyTo(cacheToPatch);

                    if (!TryValidateModel(cacheToPatch))
                    {
                        throw new InvalidOperationException("Invalid patch document");
                    }

                    // Update the original cache object in the dictionary
                    _mapper.Map(cacheToPatch, existingCache);
                });
                // return ApiControllerUtilities.HandleInvalidModelState(HttpStatusCode.BadRequest, ModelState, new List<string>() { "Unable to apply patch." });
            }
            catch (Exception ex)
            {
                return ApiControllerUtilities.HandleInvalidModelState(HttpStatusCode.BadRequest, ModelState, new List<string>() { "Unable to apply patch." });
            }
            return NoContent();
        }
    }
}
