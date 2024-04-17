using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameServerAPI.Models.API;
using GameServerAPI.Models.DTO.GameServer;
using GameServerAPI.Models.DTO.PlayerJoinToken;
using GameServerAPI.Models.TokenObjects;
using GameServerAPI.Repository.IRepository;
using GameServerAPI.Services;
using GameServerAPI.Services.Auth.IServices;
using GameServerAPI.Services.Token.IServices;
using GameServerAPI.Utilities;
using System.Net;
using System.Security.Claims;

namespace GameServerAPI.Controllers
{
    /// <summary>
    /// Controller that provides players with the ability to generate tokens
    /// to authenticate themselves to game servers.
    /// </summary>
    [Produces("application/json")]
    [Route("api/player-tokens")]
    [ApiController]
    public class PlayerTokenController : Controller
    {

        private readonly IUserRepository _userRepository;
        private ITokenService<PlayerJoinToken> _playerJoinTokenService;
        private readonly IGameServerRepository _gameServerRepository;

        /// <summary>
        /// Initializes instance of PlayerTokenController.
        /// </summary>
        /// <param name="playerJoinTokenService">An ITokenService instance used for creation of player join tokens.</param>
        /// <param name="userRepository">An IUserRepository instance used for accessing user data.</param>
        /// <param name="gameServerRepository">An IGameServerRepository instance used for accessing game server data.</param>

        public PlayerTokenController(ITokenService<PlayerJoinToken> playerJoinTokenService, IUserRepository userRepository, IGameServerRepository gameServerRepository)
        {
            _playerJoinTokenService = playerJoinTokenService;
            _userRepository = userRepository;
            _gameServerRepository = gameServerRepository;
        }
        /// <summary>
        /// Generates a unique token that the player will hand off to game servers when joining
        /// </summary>
        /// <param name="requestDTO">Contains information about the game server the player desires to join.</param>
        /// <returns>An ActionResult containing an API Response. If successful, <see cref="APIResponse.Result"/>
        /// will contain the PlayerJoinToken
        /// </returns>
        /// <response code="200">Successful. Stores player join JWT token in APIResponse.Result.</response>
        [Authorize(Roles = "Player")]
        [HttpPost("generate-token")]
        [ProducesResponseType(typeof(Models.API.SwaggerExample.APIResponse<string>), 200)]
        public async Task<ActionResult<APIResponse>> GenerateToken([FromBody] PlayerTokenRequestDTO requestDTO)
        {
            // Recieve users id
            var id_claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (id_claim == null)
            {
                return ApiControllerUtilities.HandleError(HttpStatusCode.BadRequest, "Unable to extract claim from auth token.");

            }
            // Ensure id is associated with user
            var user = await _userRepository.GetAsync(u => u.Id == id_claim.Value);
            if (user == null)
            {
                return ApiControllerUtilities.HandleError(HttpStatusCode.BadRequest, "Unable to get user associated with auth token.");
            }

            // Ensure desired game server is registered
            var gameServer = await _gameServerRepository.GetAsync(x => x.Id == requestDTO.ServerID);
            if (gameServer==null)
            {
                return ApiControllerUtilities.HandleError(HttpStatusCode.BadRequest, "Invalid game server.");
                
            }

            // Generate JWT token, and return it in response
            string token = _playerJoinTokenService.CreateToken(new PlayerJoinToken() { Id = id_claim.Value, ServerId = gameServer.Id });
            return ApiControllerUtilities.OkResult(token);

        }
    }
}
