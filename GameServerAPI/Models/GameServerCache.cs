using System.ComponentModel.DataAnnotations;

namespace GameServerAPI.Models
{
    /// <summary>
    /// Represents a cache for storing in-memory properties of a game server.
    /// </summary>
    public class GameServerCache
    {
        /// <summary>
        /// Timestamp of the last heartbeat received from the game server.
        /// </summary>
        public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Current nnumber of players connected to a game server.
        /// -1 is undefined.
        /// </summary>
        [Range(-1, int.MaxValue)]
        public int PlayerCount { get; set; } = -1;
    }
}
