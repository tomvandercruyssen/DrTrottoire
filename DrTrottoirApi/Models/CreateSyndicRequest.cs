using System.ComponentModel.DataAnnotations;

namespace DrTrottoirApi.Models
{
    public class CreateSyndicRequest
    {
        [StringLength(45)]
        [Required]
        public string? FirstName { get; set; }

        [StringLength(45)]
        [Required]
        public string? LastName { get; set; }

        [RegularExpression("^([0-9]{10})$")]
        [Required]
        public string? TelephoneNumber { get; set; }
    }
}
