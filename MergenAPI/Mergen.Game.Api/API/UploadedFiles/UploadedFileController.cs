using Mergen.Core.Managers;
using Mergen.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Mergen.Game.Api.API.UploadedFiles
{
    [ApiController]
    public class UploadedFileController : ApiControllerBase
    {
        private readonly IFileService _fileService;
        private readonly FileManager _fileManager;

        public UploadedFileController(IFileService fileService, FileManager fileManager)
        {
            _fileService = fileService;
            _fileManager = fileManager;
        }

        [HttpGet]
        [Route("files/{fileId}/data")]
        public async Task<ActionResult> GetData([FromRoute] string fileId, CancellationToken cancellationToken)
        {
            var file = await _fileManager.GetFileByFileIdAsync(fileId, cancellationToken);
            if (file == null)
                return NotFound();

            return File(_fileService.GetFile(file.FileId), file.MimeType, true);
        }
    }
}