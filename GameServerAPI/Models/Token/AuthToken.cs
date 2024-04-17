namespace GameServerAPI.Models.Token
{
    /// <summary>
    /// Class that represents claims needed for authentication tokens used in user authorization.
    /// </summary>
    public class AuthToken
    {
        /// <summary>Unique Id associated with the user.</summary>
        public string NamedId { get; set; }

        /// <summary>Role of the user.</summary>
        public string Role {  get; set; }
    }
}
