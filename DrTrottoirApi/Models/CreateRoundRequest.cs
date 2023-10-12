using System.ComponentModel.DataAnnotations;

namespace DrTrottoirApi.Models
{
    public class CreateRoundRequest
    {
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public string Name { get; set; }
        public Guid UserId { get; set; }
    }
}
