using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DrTrottoirApi.Models
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string JwtToken { get; set; }
        public ICollection<string> Roles { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }
    }
}
