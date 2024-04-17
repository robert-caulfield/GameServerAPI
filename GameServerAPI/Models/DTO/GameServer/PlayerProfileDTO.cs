namespace GameServerAPI.Models.DTO.GameServer
{
    /// <summary>
    /// Data transfer object representing a player's profile
    /// </summary>
    /// <remarks>
    /// This DTO is used to hold data about a player that just joined a game server.
    /// This can store various player related data such as cosmetics, level, or any 
    /// other data the server should know about the player.
    /// </remarks>
    public class PlayerProfileDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }

    }
}
