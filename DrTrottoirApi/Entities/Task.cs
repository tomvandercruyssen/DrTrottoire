using System.ComponentModel.DataAnnotations;

namespace DrTrottoirApi.Entities
{
    public class Task
    {
        public Guid Id { get; set; }
        public int OrderNumber { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public TimeSpan TaskDuration { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
        
        [StringLength(500)]
        public string? Remark { get; set; }
        public virtual List<Picture> Pictures { get; set; }
        public virtual Guid RoundId { get; set; }
        public virtual Round Round { get; set; }
        public virtual Company Company { get; set; }
    }
}
