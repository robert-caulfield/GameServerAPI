using System.ComponentModel.DataAnnotations;

namespace GameServerAPI.Models.DTO.GameServer
{
    public class PlayerDisconnectDTO
    {
        [Required(ErrorMessage = "PlayerId of disconnected player is required.")]
        public string PlayerId { get; set; }
    }
}
