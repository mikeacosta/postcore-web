using Postcore.Web.Core.ApiModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Postcore.Web.Core.Interfaces
{
    public interface IAdApiClient
    {
        Task<List<Ad>> GetAllAsync();
        Task<CreateResponse> CreateAsync(Ad ad);
        Task<bool> ConfirmAsync(ConfirmRequest model);
    }
}
