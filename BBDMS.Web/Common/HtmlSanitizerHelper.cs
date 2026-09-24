using System;
using System.Text.RegularExpressions;

namespace BBDMS.Web.Common
{
    public static class HtmlSanitizerHelper
    {
        private static readonly Regex ScriptRegex = new Regex(@"<script[^>]*>[\s\S]*?</script>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex IframeRegex = new Regex(@"<iframe[^>]*>[\s\S]*?</iframe>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ObjectRegex = new Regex(@"<(object|embed|applet)[^>]*>[\s\S]*?</\1>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex EventHandlersRegex = new Regex(@"\son\w+\s*=\s*(['""]).*?\1", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex JavascriptHrefRegex = new Regex(@"href\s*=\s*(['""])javascript:.*?\1", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string Sanitize(string? rawHtml)
        {
            if (string.IsNullOrWhiteSpace(rawHtml))
                return string.Empty;

            var sanitized = rawHtml;
            sanitized = ScriptRegex.Replace(sanitized, string.Empty);
            sanitized = IframeRegex.Replace(sanitized, string.Empty);
            sanitized = ObjectRegex.Replace(sanitized, string.Empty);
            sanitized = EventHandlersRegex.Replace(sanitized, string.Empty);
            sanitized = JavascriptHrefRegex.Replace(sanitized, "href=\"#\"");

            return sanitized;
        }
    }
}
