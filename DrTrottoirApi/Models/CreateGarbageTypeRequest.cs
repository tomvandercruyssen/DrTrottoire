using System.ComponentModel.DataAnnotations;

namespace DrTrottoirApi.Models
{
    public class CreateGarbageTypeRequest
    {
        [StringLength(45)]
        [Required]
        public string? Name { get; set; }
    }
}
