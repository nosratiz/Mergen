using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mergen.Core.Services
{
    public class FileService : IFileService
    {
        public async Task SaveFile(Stream file, string path, string fileName, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, 81920, cancellationToken);
            }
        }

        public Stream GetFile(string path, string fileName)
        {
            var filePath = Path.Combine(path, fileName);
            return File.OpenRead(filePath);
        }

        public byte[] GetFileAsArrayOfBytes(string path, string fileName)
        {
            var filePath = Path.Combine(path, fileName);
            return File.ReadAllBytes(filePath);
        }

        public void DeleteFile(string path, string fileName)
        {
            var filePath = Path.Combine(path, fileName);
            File.Delete(filePath);
        }

        public long GetFileSize(string path, string fileName)
        {
            return new FileInfo(Path.Combine(path, fileName)).Length;
        }
    }
}