using DrTrottoirApi.Entities;

namespace DrTrottoirApi.Models
{
    public class BaseRoundResponse
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Name { get; set; }
        public RoundStatus Status { get; set; }
        public Guid UserId { get; set; }
    }
}
