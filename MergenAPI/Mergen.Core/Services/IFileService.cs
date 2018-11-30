using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mergen.Core.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(Stream file, CancellationToken cancellationToken);
        Stream GetFile(string id);
        void DeleteFile(string id);
        long GetFileSize(string id);
    }
}