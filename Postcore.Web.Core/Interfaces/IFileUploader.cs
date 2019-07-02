using System.IO;
using System.Threading.Tasks;


namespace Postcore.Web.Core.Interfaces
{
    public interface IFileUploader
    {
        Task<bool> UploadFileAsync(string fileName, Stream storageStream);
    }
}
