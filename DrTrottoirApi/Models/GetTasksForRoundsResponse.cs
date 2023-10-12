using DrTrottoirApi.Entities;
using TaskStatus = DrTrottoirApi.Entities.TaskStatus;

namespace DrTrottoirApi.Models
{
    public class GetTasksForRoundsResponse
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public TaskStatus Status { get; set; }
        public IList<GarbageType> GarbageTypesInside { get; set; }
        public IList<GarbageType> GarbageTypesOutside { get; set; }

    }
}
