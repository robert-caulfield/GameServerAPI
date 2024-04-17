namespace GameServerAPI.Models.DTO.GameServer
{
    /// <summary>
    /// DTO that represents a registered game server.
    /// </summary>
    public class GameServerDetailsDTO
    {
        /// <summary>Id of game server.</summary>
        /// <example>ABC32454</example>
        public string Id { get; set; }

        /// <summary>Name of game server.</summary>
        /// <example>NA East 1</example>
        public string Name { get; set; }

        /// <summary>IP address of game server.</summary>
        /// <example>192.168.1.1</example>
        public string IPAddress { get; set; }

        /// <summary>Port of game server.</summary>
        /// <example>25565</example>
        public int Port { get; set; }

        /// <summary>Number of players current connected to the game server.</summary>
        /// <example>0</example>
        public int PlayerCount { get; set; }

        /// <summary>Max number of players allowed to be connected to game server.</summary>
        /// <example>8</example>
        public int MaxPlayers { get; set; }
    }
}
