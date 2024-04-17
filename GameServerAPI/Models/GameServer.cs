using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServerAPI.Models
{
    /// <summary>
    /// Represents a GameServer with its properties and methods
    /// </summary>
    public class GameServer
    {
        /// <summary> Unique identifier for the game server.</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Name of game server.</summary>
        [Required]
        public string Name { get; set; }

        /// <summary>IP address of the game server.</summary>
        [Required]
        public string IPAddress { get; set; }

        /// <summary>Port of game server.</summary>
        [Required]
        [Range(0, 65535)]
        public int Port { get; set; }

        /// <summary>Max number of players allowed on a game server.</summary>
        [Required]
        public int MaxPlayers { get; set; }

        /// <summary>Foreign key and navigation property to ApplicationUser that is creator</summary>
        [ForeignKey("Creator")]
        [Required]
        public string CreatorUserId { get; set; }

        [ForeignKey("CreatorUserId")]
        public virtual ApplicationUser Creator { get; set; }

    }
}
