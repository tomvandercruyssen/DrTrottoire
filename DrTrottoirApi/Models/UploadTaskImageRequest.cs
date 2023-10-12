using System.ComponentModel.DataAnnotations;
using DrTrottoirApi.Entities;

namespace DrTrottoirApi.Models
{
    public class UploadTaskImageRequest
    {
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public PictureLabel PictureLabel { get; set; }
        [Required]
        public IFormFile Image { get; set; }
    }
}
