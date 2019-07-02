using Postcore.AdApi.Shared.Models;

namespace Postcore.Web.Core.ApiModels
{
    public class ConfirmRequest
    {
        public string Id { get; set; }
        public string FilePath { get; set; }
        public AdStatus Status { get; set; }
    }
}
