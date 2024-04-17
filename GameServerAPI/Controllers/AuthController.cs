using Microsoft.AspNetCore.Mvc;
using GameServerAPI.Models.API;
using GameServerAPI.Models.DTO.Auth;
using GameServerAPI.Services;
using GameServerAPI.Services.Auth.IServices;
using GameServerAPI.SwaggerExamples;
using GameServerAPI.Utilities;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace GameServerAPI.Controllers
{
    /// <summary>
    /// Controller responsible for handling authentication related requests, such as
    /// user login and registration. 
    /// </summary>
    [Produces("application/json")]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes instance of GameServerController.
        /// </summary>
        /// <param name="authService">An instance of IAuthService used for user authentication methods.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user based on their login credentials.
        /// </summary>
        /// <param name="loginRequestDTO">The login request containing the user's credentials.</param>
        /// <returns>
        /// An ActionResult containing an APIResponse. If successful, <see cref="APIResponse.Result"/>
        /// will contain the JWT authentication token.
        /// </returns>
        /// <response code="200">Login successful. Stores LoginResponseDTO in APIResponse.Result.</response>
        /// <response code="400">There was a problem logging in with provided login information.</response>

        [HttpPost("login")]
        [ProducesResponseType(typeof(Models.API.SwaggerExample.APIResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]


        public async Task<ActionResult<APIResponse>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            //Check valid model state
            if (!ModelState.IsValid)
            {
                return ApiControllerUtilities.HandleInvalidModelState(HttpStatusCode.BadRequest, ModelState);
            }

            // Login with auth service and return result
            ServiceResult<LoginResponseDTO> serviceResponse = await _authService.Login(loginRequestDTO);
            if (serviceResponse.IsSuccess) {
                // Ok result API response containing a LoginResponseDTO as the result
                return ApiControllerUtilities.OkResult(serviceResponse.Result);
            }
            else
            {
                return ApiControllerUtilities.HandleError(serviceResponse.StatusCode, serviceResponse.Errors);
            }
        }

        /// <summary>
        /// Registers a new user with the provided registration information.
        /// </summary>
        /// <param name="registrationRequestDTO">The registration request containing the user's information.</param>
        /// <returns>
        /// An ActionResult containing an APIResponse. If successful, <see cref="APIResponse.IsSuccess"/>
        /// will indicate successful registration.
        /// </returns>
        /// <response code="200">Registration successful. Stores UserDTO in APIResponse.Result</response>
        /// <response code="400">Could not register new user with provided information.</response>
        /// <response code="500">A server error was ecountered in registration process.</response>
        
        [HttpPost("register")]
        [ProducesResponseType(typeof(Models.API.SwaggerExample.APIResponse<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(ServerErrorExample))]
        public async Task<ActionResult<APIResponse>> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            //Check valid model state
            if (!ModelState.IsValid)
            {
                return ApiControllerUtilities.HandleInvalidModelState(HttpStatusCode.BadRequest, ModelState);
            }

            // Register with auth service and return result
            ServiceResult<UserDTO> serviceResponse = await _authService.Register(registrationRequestDTO);
            if (serviceResponse.IsSuccess)
            {
                // Ok result API response containing a UserDTO as the result
                return ApiControllerUtilities.OkResult(serviceResponse.Result);
            }
            else
            {
                return ApiControllerUtilities.HandleError(serviceResponse.StatusCode, serviceResponse.Errors);
            }
        }
    }
}
