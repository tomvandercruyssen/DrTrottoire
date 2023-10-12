using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DrTrottoirApi.Entities
{
    public class Role: IdentityRole<Guid>
    {
        [StringLength(45)]  
        public string Description { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
