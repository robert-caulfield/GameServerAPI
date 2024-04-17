using System.ComponentModel.DataAnnotations;

namespace GameServerAPI.Models.DTO.Auth
{
    /// <summary>
    /// DTO that holds information required for a registration request.
    /// </summary>
    public class RegistrationRequestDTO
    {
        /// <summary>Username of new user.</summary>
        /// <example>Player25</example>
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        /// <summary>Email of new user.</summary>
        /// <example>Player25@example.com</example>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        /// <summary>Password of new user.</summary>
        /// <example>Password123$</example>
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
