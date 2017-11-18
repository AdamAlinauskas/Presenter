using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Meetaroo.Services
{
    public class CookieManager : ICookieManager
    {
        private ChunkingCookieManager manager;

        public CookieManager()
        {
            manager = new ChunkingCookieManager();
        }

        public void AppendResponseCookie(HttpContext context, string key, string value, CookieOptions options)
        {
            GenericizeDomain(context, options);

            manager.AppendResponseCookie(context, key, value, options);
        }

        public void DeleteCookie(HttpContext context, string key, CookieOptions options)
        {
            GenericizeDomain(context, options);

            manager.DeleteCookie(context, key, options);
        }

        private void GenericizeDomain(HttpContext context, CookieOptions options)
        {
            // TODO : Duplicated code from RetrieveSchemaActionFilter
            var hostnameRegex = new Regex(@"^(?<schema>[^\.]+)\.(?<host>[^\.]+\.[^\.]+)$");
            string hostname = context.Request.Host.Host;
            var match = hostnameRegex.Match(hostname);
            var host = "." + (match.Success ? match.Groups["host"].Value : hostname);

            if (options.Domain == null || options.Domain.EndsWith(host)) {
                options.Domain = host;
            }
        }

        public string GetRequestCookie(HttpContext context, string key)
        {
            return manager.GetRequestCookie(context, key);
        }
    }
}