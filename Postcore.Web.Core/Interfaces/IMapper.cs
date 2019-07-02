using Postcore.Web.Core.ApiModels;
using Postcore.Web.Core.WebModels.AdManagement;

namespace Postcore.Web.Core.Interfaces
{
    public interface IMapper
    {
        AdApi.Shared.Models.AdDto ToAdDto(Ad ad);
        CreateResponse ToCreateResponse(AdApi.Shared.Models.CreateAdResponseDto response);
        AdApi.Shared.Models.ConfirmAdDto ToConfirmAdDto(ConfirmRequest request);
        Ad ToAd(CreateAdViewModel model);
    }
}
