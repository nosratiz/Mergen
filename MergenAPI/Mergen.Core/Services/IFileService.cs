using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mergen.Core.Services
{
    public interface IFileService
    {
        Task SaveFile(Stream file, string path, string fileName, CancellationToken cancellationToken);
        Stream GetFile(string path, string fileName);
        byte[] GetFileAsArrayOfBytes(string path, string fileName);
        void DeleteFile(string path, string fileName);
        long GetFileSize(string path, string fileName);
    }
}