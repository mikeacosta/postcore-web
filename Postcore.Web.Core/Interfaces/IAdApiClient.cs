using Postcore.Web.Core.ApiModels;
using System.Threading.Tasks;

namespace Postcore.Web.Core.Interfaces
{
    public interface IAdApiClient
    {
        Task<CreateResponse> CreateAsync(Ad ad);
        Task<bool> ConfirmAsync(ConfirmRequest model);
    }
}
