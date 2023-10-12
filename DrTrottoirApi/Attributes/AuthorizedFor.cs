using DrTrottoirApi.Entities;
using Microsoft.AspNetCore.Authorization;

namespace DrTrottoirApi.Attributes
{
    public class AuthorizedFor : AuthorizeAttribute
    {
        public AuthorizedFor(params Roles[] roles)
        {
            Roles = string.Join(", ", roles);
        }
    }
}
