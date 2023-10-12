using System.ComponentModel.DataAnnotations;

namespace DrTrottoirApi.Entities
{
    public class Company
    {
        public Guid Id { get; set; }

        [Required]
        public string IdKbo { get; set; }

        [StringLength(255)]
        [Required]
        public string Name { get; set; }
        [StringLength(255)]
        [Required]
        public string Address { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string? PictureUrl { get; set; }
        public string? ManualUrl { get; set; }

        [Required]
        public virtual Syndic Syndic { get; set; }
    }
}
