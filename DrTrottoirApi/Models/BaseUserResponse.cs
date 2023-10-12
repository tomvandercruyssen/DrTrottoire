using DrTrottoirApi.Entities;

namespace DrTrottoirApi.Models
{
    public class BaseUserResponse
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? PictureUrl { get; set; }
        public IList<string>? Roles { get; set; }
        public WorkArea? WorkArea { get; set; }
    }
}
