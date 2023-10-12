using System.ComponentModel.DataAnnotations;

namespace DrTrottoirApi.Models
{
    public class UploadCompanyManualRequest
    {
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public IFormFile Manual { get; set; }
    }
}
