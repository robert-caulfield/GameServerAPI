namespace GameServerAPI.Models.TokenObjects
{
    /// <summary>
    /// Class that represents claims used in player join tokens.
    /// </summary>
    public class PlayerJoinToken
    {
        /// <summary>Id of requesting player.</summary>
        public string Id { get; set; }

        /// <summary>Id of the game server the user desires to join.</summary>
        public string ServerId { get; set; }
    }
}
