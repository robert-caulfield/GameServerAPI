namespace GameServerAPI.Models.DTO.Auth
{
    /// <summary>
    /// DTO that is stored in response after a successful login request
    /// </summary>
    public class LoginResponseDTO
    {
        /// <summary>DTO containing details about a user.</summary>
        public UserDTO User { get; set; }

        /// <summary>Role of signed in user.</summary>
        /// <example>Player</example>
        public string Role { get; set; }

        /// <summary>Token used to authorize future API requests.</summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c</example>
        public string Token { get; set; }
    }
}
