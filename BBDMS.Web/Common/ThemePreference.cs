using System;
using Microsoft.AspNetCore.Http;

namespace BBDMS.Web.Common
{
    /// <summary>
    /// Owns the theme cookie so the public site, donor portal and admin panel all
    /// resolve the same light/dark value. The cookie carries the choice for anonymous
    /// visitors; signed-in donors and admins additionally persist it on their record.
    /// </summary>
    public static class ThemePreference
    {
        public const string CookieName = "bbdms.theme";
        public const string Light = "light";
        public const string Dark = "dark";

        public static string Normalize(string value)
        {
            return string.Equals(value?.Trim(), Dark, StringComparison.OrdinalIgnoreCase) ? Dark : Light;
        }

        public static string Read(HttpContext httpContext)
        {
            var cookie = httpContext?.Request?.Cookies[CookieName];
            return string.IsNullOrWhiteSpace(cookie) ? Light : Normalize(cookie);
        }

        public static void ApplyCookie(HttpContext httpContext, string theme)
        {
            if (httpContext?.Response == null) return;

            httpContext.Response.Cookies.Append(CookieName, Normalize(theme), new CookieOptions
            {
                HttpOnly = false, // the theme script keeps the cookie in sync without a round trip
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = httpContext.Request.IsHttps,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
        }
    }
}
