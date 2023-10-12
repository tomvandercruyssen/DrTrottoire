using System.ComponentModel.DataAnnotations;
using DrTrottoirApi.Entities;
using TaskStatus = DrTrottoirApi.Entities.TaskStatus;

namespace DrTrottoirApi.Models
{
    public class CreateTaskRequest
    {
        public Guid RoundId { get; set; }
        public Guid CompanyId { get; set; }
    }
}
