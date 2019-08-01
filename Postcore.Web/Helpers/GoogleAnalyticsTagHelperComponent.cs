using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System;

namespace Postcore.Web.Helpers
{
    public class GoogleAnalyticsTagHelperComponent : TagHelperComponent
    {
        private readonly GoogleAnalyticsSettings _settings;

        public GoogleAnalyticsTagHelperComponent(IOptions<GoogleAnalyticsSettings> settings)
        {
            _settings = settings.Value;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.Equals(output.TagName, "head", StringComparison.OrdinalIgnoreCase))
            {
                var trackingCode = _settings.TrackingCode;
                if (!string.IsNullOrEmpty(trackingCode))
                {
                    output.PostContent
                        .AppendHtml("<script async src='https://www.googletagmanager.com/gtag/js?id=")
                        .AppendHtml(trackingCode)
                        .AppendHtml("'></script>")
                        .AppendHtml("<script>window.dataLayer = window.dataLayer || [];")
                        .AppendHtml("function gtag(){dataLayer.push(arguments);}")
                        .AppendHtml("gtag('js', new Date());")
                        .AppendHtml("gtag('config', '")
                        .AppendHtml(trackingCode)
                        .AppendHtml("');")
                        .AppendHtml("</script>");
                }
            }
        }
    }
}
