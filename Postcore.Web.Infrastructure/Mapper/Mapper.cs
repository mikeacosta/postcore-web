using Postcore.AdApi.Shared.Models;
using Postcore.Web.Core.ApiModels;
using Postcore.Web.Core.Interfaces;
using Postcore.Web.Core.WebModels.AdManagement;

namespace Postcore.Web.Infrastructure.Mapper
{
    public class Mapper : IMapper
    {
        public Ad ToAd(CreateAdViewModel model)
        {
            return new Ad
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price
            };
        }

        public Ad ToAd(AdDto dto)
        {
            return new Ad
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Username = dto.Username
            };
        }

        public AdApi.Shared.Models.AdDto ToAdDto(Ad ad)
        {
            return new AdApi.Shared.Models.AdDto
            {
                Id = ad.Id,
                Title = ad.Title,
                Description = ad.Description,
                Price = ad.Price,
                Username = ad.Username
            };
        }

        public AdApi.Shared.Models.ConfirmAdDto ToConfirmAdDto(ConfirmRequest request)
        {
            return new AdApi.Shared.Models.ConfirmAdDto()
            {
                Id = request.Id,
                FilePath = request.FilePath,
                Status = request.Status
            };
        }

        public CreateResponse ToCreateResponse(AdApi.Shared.Models.CreateAdResponseDto response)
        {
            return new CreateResponse
            {
                Id = response.Id
            };
        }
    }
}
