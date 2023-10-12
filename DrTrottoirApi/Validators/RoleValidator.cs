using DrTrottoirApi.Entities;

namespace DrTrottoirApi.Validators
{
    public static class RoleValidator
    {
        public static bool CorrectRoles(List<string> roles)
        {
            foreach (var role in roles)
            {
                if (!Enum.IsDefined(typeof(Roles), role))
                    return false;
            }

            return true;
        }
    }
}
