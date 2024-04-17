using System.ComponentModel.DataAnnotations;

namespace GameServerAPI.Models.DTO.Auth
{
    /// <summary>
    /// DTO that holds information required to sign in
    /// </summary>
    public class LoginRequestDTO
    {
        /// <summary>Username of desired user.</summary>
        /// <example>Player25</example>
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        /// <summary>Password of desired user.</summary>
        /// <example>Password123$</example>
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
