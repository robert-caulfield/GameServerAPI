namespace GameServerAPI.Models.DTO.Auth
{
    /// <summary>
    /// DTO that holds user information that is sent back after successful
    /// registration.
    /// </summary>
    public class UserDTO
    {
        /// <summary>ID of the user</summary>
        /// <example>1764DSF345tDF45</example>
        public string ID { get; set; }

        /// <summary>Username of the user</summary>
        /// <example>Player25</example>
        public string UserName { get; set; }
    }
}
