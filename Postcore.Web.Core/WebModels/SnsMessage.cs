using System.Collections.Generic;

namespace Postcore.Web.Core.WebModels
{
    public class SnsMessage
    {
        public List<string> Body { get; set; } = new List<string>();

        public string StatusCode { get; set; }
    }
}
