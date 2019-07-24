using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Postcore.Web.Attributes
{
    public class RequireWwwAttribute : Attribute, IAuthorizationFilter, IOrderedFilter
    {
        public int Order { get; set; }

        public bool Permanent
        {
            get => _permanent ?? true;
            set => _permanent = value;
        }

        private bool? _permanent;

        public bool IgnoreLocalhost
        {
            get => _ignoreLocalhost ?? true;
            set => _ignoreLocalhost = value;
        }

        private bool? _ignoreLocalhost;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var req = context.HttpContext.Request;
            var host = req.Host;

            var isLocalHost = string.Equals(host.Host, "localhost", StringComparison.OrdinalIgnoreCase);
            if (IgnoreLocalhost && isLocalHost)
                return;

            if (host.Host.StartsWith("www", StringComparison.OrdinalIgnoreCase))
                return;

            var optionsAccessor = context.HttpContext.RequestServices.GetRequiredService<IOptions<MvcOptions>>();
            var permanentValue = _permanent?? optionsAccessor.Value.RequireHttpsPermanent;

            var newPath = $"{req.Scheme}://www.{host.Value}{ req.PathBase}{ req.Path}{ req.QueryString}";
            context.Result = new RedirectResult(newPath, permanentValue);
        }
    }
}
