// Copyrights(c) Charqe.io. All rights reserved.

using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ResourceLocalizer.Helpers
{
    public static class LocalizationHelper
    {
        /// <summary>
        /// We need accessor to get stored culture language information.
        /// If you store lang somewhere else, you can get it from other ways and can remove HttpContextAccessor.
        /// </summary>
        private static readonly IHttpContextAccessor _accessor = new HttpContextAccessor();

        /// <summary>
        /// This is a HtmlHelper method. We are using this on razor views.
        /// Usage: @Html.Localise("Title")
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Localise(this IHtmlHelper helper, string key)
        {
            using var rm = new ResourceManagement();
            
            // lang can store somewhere else.
            var lang = _accessor.HttpContext.Request.Cookies["lang"];

            var res = rm.GetString(key ?? string.Empty, CultureInfo.CreateSpecificCulture(lang ?? "tr-TR")) ?? $"~{key}~";
            return res;
        }
    }
}