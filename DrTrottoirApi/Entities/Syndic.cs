using System.ComponentModel.DataAnnotations;

namespace DrTrottoirApi.Entities
{
    public class Syndic
    {
        public Guid Id { get; set; }

        [StringLength(45)]
        [Required]
        public string? FirstName { get; set; }

        [StringLength(45)]
        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? TelephoneNumber { get; set; }
    }
}
