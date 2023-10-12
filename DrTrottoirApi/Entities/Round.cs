using Microsoft.Build.Framework;

namespace DrTrottoirApi.Entities
{
    public class Round
    {
        public Guid Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public string Name { get; set; }
        public RoundStatus Status { get; set; } = RoundStatus.NotStarted;
        public TimeSpan RoundDuration { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
