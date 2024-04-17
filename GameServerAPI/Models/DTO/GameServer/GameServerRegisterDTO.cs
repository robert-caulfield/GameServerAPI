using System.ComponentModel.DataAnnotations;

namespace GameServerAPI.Models.DTO.GameServer
{
    public class GameServerRegisterDTO
    {
        /// <summary>Name of game server</summary>
        /// <example>NA East 1</example>
        [Required(ErrorMessage = "Server name is required.")]
        public string Name { get; set; }

        /// <summary>IPAddress of game server</summary>
        /// <example>192.168.1.1</example>
        [Required]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Invalid IP Address")]
        public string IPAddress { get; set; }

        /// <summary>Port of game server</summary>
        /// <example>25565</example>
        [Required]
        [Range(1,65535, ErrorMessage = "Port must be between 1 and 65535")]
        public int Port { get; set; }

        /// <summary>Max amount of players allowed in game server.</summary>
        /// <remarks>Default is 8</remarks>
        /// <example>8</example>
        public int MaxPlayers { get; set; } = 8;
    }
}
