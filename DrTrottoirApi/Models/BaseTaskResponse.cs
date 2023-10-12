using DrTrottoirApi.Entities;
using TaskStatus = DrTrottoirApi.Entities.TaskStatus;

namespace DrTrottoirApi.Models
{
    public class BaseTaskResponse
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public TaskStatus Status { get; set; }
        public IList<BasePictureResponse> BasePictureResponses { get; set; }
        public string? Remark { get; set; }
        public Guid RoundId { get; set; }
        public Guid CompanyId { get; set; }
    }
}
