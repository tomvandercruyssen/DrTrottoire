using DrTrottoirApi.Entities;
using Microsoft.Build.Framework;

namespace DrTrottoirApi.Models
{
    public class DeleteGarbageTypeRequest
    {
        [Required]
        public int GarbageTypeId { get; set; }
        [Required]
        public Guid CompanyId { get; set; }
        public DateTime? DateToDelete { get; set; } = null;
    }
}
