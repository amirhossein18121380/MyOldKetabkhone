using System.Security.Claims;
using Common.Extension;

namespace Common.Extension
{
    public static class JwtExtension
    {
        public static long? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToLong();
        }
    }
}
