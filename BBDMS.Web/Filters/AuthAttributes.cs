using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;

namespace BBDMS.Web.Filters
{
    // Roles
    public static class Roles
    {
        public const string Donor = "Donor";
        public const string Admin = "Admin";
    }

    // Role-based Authorization Attributes
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public AdminAuthorizeAttribute() => Roles = BBDMS.Web.Filters.Roles.Admin;
    }

    public class DonorAuthorizeAttribute : AuthorizeAttribute
    {
        public DonorAuthorizeAttribute() => Roles = BBDMS.Web.Filters.Roles.Donor;
    }

    // Claims Principal Extensions
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this IPrincipal? user)
        {
            var value = (user?.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
