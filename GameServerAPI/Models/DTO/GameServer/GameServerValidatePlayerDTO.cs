using System.ComponentModel.DataAnnotations;

namespace GameServerAPI.Models.DTO.GameServer
{
    public class GameServerValidatePlayerDTO
    {
        [Required(ErrorMessage = "PlayerJoinToken is required.")]
        public string PlayerJoinToken { get; set; }
    }
}
