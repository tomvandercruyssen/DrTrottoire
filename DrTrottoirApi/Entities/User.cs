using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DrTrottoirApi.Entities
{
    public class User: IdentityUser<Guid>
    {

        [StringLength(45)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(45)]
        [Required]
        public string LastName { get; set; }

        [StringLength(45)]
        [Required]
        public string TelephoneNumber { get; set; }

        public string? PictureUrl { get; set; }

        public virtual Guid WorkAreaId { get; set; }
        public virtual WorkArea WorkArea { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
