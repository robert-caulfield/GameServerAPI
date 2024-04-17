using GameServerAPI.Models.DTO.Auth;

namespace GameServerAPI.Services.Auth.IServices
{
    /// <summary>
    /// Defines methods for authentication and user management services.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Checks if a specified username is unique.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>True if the username is unique, false if username is taken.</returns>
        Task<bool> IsUniqueUser(string username);

        /// <summary>
        /// Authenticates user based on username and password.
        /// </summary>
        /// <param name="loginRequestDTO">Login request object that contains username and password.</param>
        /// <returns>A <see cref="ServiceResult{LoginResponseDTO}"/> containing the <see cref="LoginResponseDTO"/> if login
        /// was successful.
        /// </returns>
        Task<ServiceResult<LoginResponseDTO>> Login(LoginRequestDTO loginRequestDTO);

        /// <summary>
        /// Registers a new user, grants them 'Player' role. 
        /// </summary>
        /// <param name="registrationRequestDTO">Registration request object containing details about the desired new user.</param>
        /// <returns>A <see cref="ServiceResult{UserDTO}"/> containg a <see cref="UserDTO"/> as the result if registration was
        /// successful.
        /// </returns>
        Task<ServiceResult<UserDTO>> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
