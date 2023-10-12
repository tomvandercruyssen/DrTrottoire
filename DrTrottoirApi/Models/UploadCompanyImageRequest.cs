using System.ComponentModel.DataAnnotations;
using DrTrottoirApi.Entities;

namespace DrTrottoirApi.Models
{
    public class UploadCompanyImageRequest
    {
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public IFormFile Image { get; set; }
    }
}
