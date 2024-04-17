using Azure;
using Microsoft.AspNetCore.Mvc;
using GameServerAPI.Models.API;
using GameServerAPI.Models.DTO.GameServer;
using GameServerAPI.Models;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using GameServerAPI.Utilities;
using GameServerAPI.Services;

namespace GameServerAPI.Controllers
{
    /// <summary>
    /// Provides information about all registered game servers
    /// </summary>
    [Produces("application/json")]
    [Route("api/server-browser")]
    [ApiController]
    public class ServerBrowserController : Controller
    {
        private readonly GameServerManager _gameServerManager;
        /// <summary>
        /// Initializes instance of ServerBrowserController.
        /// </summary>
        /// <param name="gameServerManager">Instance of GameServerManager used for accessing game server information.</param>
        public ServerBrowserController(GameServerManager gameServerManager)
        {
            _gameServerManager = gameServerManager;
        }

        /// <summary>
        /// Gets a list of all currently registered game servers.
        /// </summary>
        /// <returns>
        /// An ActionResult containing an APIResponse. If successful, <see cref="APIResponse.Result"/>
        /// will contain a list of <see cref="GameServerDetailsDTO"/> objects representing the registered gameservers.
        /// </returns>
        /// <response code="200">Successful. Stores list of GameServerResponseDTOs in APIResponse.Result.</response>
        [HttpGet]
        [ProducesResponseType(typeof(Models.API.SwaggerExample.APIResponse<List<GameServerDetailsDTO>>), 200)]
        public async Task<ActionResult<APIResponse>> GetServers()
        {
            List<GameServerDetailsDTO> serverList = await _gameServerManager.GetAllGameServerDetails();
            return ApiControllerUtilities.OkResult(serverList);
        }
    }
}
