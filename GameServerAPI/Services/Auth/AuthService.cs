using AutoMapper;
using Microsoft.AspNetCore.Identity;
using GameServerAPI.Models;
using GameServerAPI.Models.DTO.Auth;
using GameServerAPI.Models.Token;
using GameServerAPI.Repository.IRepository;
using GameServerAPI.Services.Auth.IServices;
using GameServerAPI.Services.Token.IServices;
using System.Net;

namespace GameServerAPI.Services.Auth
{
    /// <summary>
    /// Provides user authentication functionality including login and registration.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService<AuthToken> _tokenService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes instance of AuthService.
        /// </summary>
        /// <param name="userRepo">An IUserRepository instance used for accessing user data.</param>
        /// <param name="userManager"></param>
        /// <param name="mapper">An IMapper instance used for object mapping.</param>
        /// <param name="tokenService">An ITokenService instance to create auth tokens. </param>
        public AuthService(IUserRepository userRepo, UserManager<ApplicationUser> userManager, IMapper mapper, ITokenService<AuthToken> tokenService)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        /// <summary>
        /// Checks if a specified username is unique.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>True if the username is unique, false if username is taken.</returns>
        public async Task<bool> IsUniqueUser(string username)
        {
            var user = await _userRepo.GetAsync(x => x.UserName.ToLower() == username.ToLower());
            return user == null;
        }

        /// <summary>
        /// Authenticates user based on username and password.
        /// </summary>
        /// <param name="loginRequestDTO">Login request object that contains username and password.</param>
        /// <returns>A <see cref="ServiceResult{LoginResponseDTO}"/> containing the <see cref="LoginResponseDTO"/> if login
        /// was successful.
        /// </returns>
        public async Task<ServiceResult<LoginResponseDTO>> Login(LoginRequestDTO loginRequestDTO)
        {
            // Get user associated with username
            var user = await _userRepo.GetAsync(u => u.UserName.ToLower() == loginRequestDTO.Username.ToLower());

            // If a user with that username exists and the password matches
            bool isValid = user != null && await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if (!isValid)
            {
                return new ServiceResult<LoginResponseDTO>()
                {
                    Result = null,
                    Errors = { "Username or password is incorrect" },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            // Get the roles associated with user
            var roles = await _userManager.GetRolesAsync(user);

            // Create claims for user auth token
            AuthToken authToken = new AuthToken
            {
                NamedId = user.Id.ToString(),
                Role = roles.FirstOrDefault()
            };

            // Create and store jwt token
            var token = _tokenService.CreateToken(authToken);

            // Construct LoginResponseDTO and store user data and token
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = token,
                User = _mapper.Map<UserDTO>(user),
                Role = roles.FirstOrDefault()
            };

            return new ServiceResult<LoginResponseDTO>()
            {
                Result = loginResponseDTO
            };

        }

        /// <summary>
        /// Registers a new user, grants them 'Player' role. 
        /// </summary>
        /// <param name="registrationRequestDTO">Registration request object containing details about the desired new user.</param>
        /// <returns>A <see cref="ServiceResult{UserDTO}"/> containg a <see cref="UserDTO"/> as the result if registration was
        /// successful.
        /// </returns>
        public async Task<ServiceResult<UserDTO>> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ApplicationUser user = _mapper.Map<ApplicationUser>(registrationRequestDTO);

            // Check if username is unique
            if(!await IsUniqueUser(user.UserName))
            {
                return new ServiceResult<UserDTO>()
                {
                    Result = null,
                    Errors = {"Username is already taken."},
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            // Attempt register
            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded)
                { // Register success

                    // Assign player role
                    await _userManager.AddToRoleAsync(user, "Player");

                    // Get ApplicationUser of new user and return it
                    var userToReturn = await _userRepo.GetAsync(u => u.UserName == registrationRequestDTO.UserName);
                    return new ServiceResult<UserDTO>()
                    {
                        Result = _mapper.Map<UserDTO>(userToReturn)
                    };
                }
                else
                { // Register fail

                    // Get error messages from identity result so it can be returned
                    var errorMessages = result.Errors.Select(e => e.Description).ToList();
                    return new ServiceResult<UserDTO>()
                    {
                        Result = null,
                        Errors = errorMessages,
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
            }
            catch (Exception ex) { }

            // If an exception was thrown in the registration process
            return new ServiceResult<UserDTO>()
            {
                Result = null,
                Errors = { "A problem was encountered while registering." },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }
}
