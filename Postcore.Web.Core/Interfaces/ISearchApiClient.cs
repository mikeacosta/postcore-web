using Postcore.Web.Core.ApiModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Postcore.Web.Core.Interfaces
{
    public interface ISearchApiClient
    {
        Task<List<Ad>> Search(string keyword);
    }
}
