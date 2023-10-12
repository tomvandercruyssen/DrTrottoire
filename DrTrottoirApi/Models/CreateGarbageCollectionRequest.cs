using System.ComponentModel.DataAnnotations;
using DrTrottoirApi.Entities;

namespace DrTrottoirApi.Models
{
    public class CreateGarbageCollectionRequest
    {
        public Guid CompanyId { get; set; }
        [Required]
        public DateTime CollectionTime { get; set; }
        [Required]
        public List<string>? GarbageTypes { get; set; }

        public bool HasToBeBroughtOutside { get; set; }
        public bool SetForWholeMonth { get; set; } = false;
    }
}
