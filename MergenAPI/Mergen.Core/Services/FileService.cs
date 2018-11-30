using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using FileOptions = Mergen.Core.Options.FileOptions;

namespace Mergen.Core.Services
{
    public class FileService : IFileService
    {
        private readonly FileOptions _options;

        public FileService(IOptions<FileOptions> options)
        {
            _options = options.Value;
            Directory.CreateDirectory(_options.BaseStoragePath);
        }

        public async Task<string> SaveFileAsync(Stream file, CancellationToken cancellationToken)
        {
            var fileName = Guid.NewGuid().ToString("N");
            var filePath = Path.Combine(_options.BaseStoragePath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, 81920, cancellationToken);
            }

            return fileName;
        }

        public Stream GetFile(string id)
        {
            var filePath = Path.Combine(_options.BaseStoragePath, id);
            return File.OpenRead(filePath);
        }

        public void DeleteFile(string id)
        {
            var filePath = Path.Combine(_options.BaseStoragePath, id);
            File.Delete(filePath);
        }

        public long GetFileSize(string id)
        {
            return new FileInfo(Path.Combine(_options.BaseStoragePath, id)).Length;
        }
    }
}