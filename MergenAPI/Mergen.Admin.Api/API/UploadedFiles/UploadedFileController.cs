using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Admin.Api.Helpers;
using Mergen.Api.Core.ViewModels;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Managers;
using Mergen.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Admin.Api.API.UploadedFiles
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

        [HttpPost]
        [Route("files")]
        public async Task<ActionResult<ApiResultViewModel<UploadedFileViewModel>>> UploadFile([FromForm]FileInputModel inputModel,
            CancellationToken cancellationToken)
        {
            string fileId;
            using (var stream = inputModel.File.OpenReadStream())
            {
                fileId = await _fileService.SaveFileAsync(stream, cancellationToken);
            }

            var mimeTypeCategory = inputModel.File.ContentType.Split('/')[0];
            int mimeTypeCategoryId;

            if (mimeTypeCategory.Equals("image", StringComparison.OrdinalIgnoreCase))
                mimeTypeCategoryId = UploadedFileMimeTypeCategoryIds.Image;
            else if (mimeTypeCategory.Equals("audio", StringComparison.OrdinalIgnoreCase))
                mimeTypeCategoryId = UploadedFileMimeTypeCategoryIds.Audio;
            else if (mimeTypeCategory.Equals("video", StringComparison.OrdinalIgnoreCase))
                mimeTypeCategoryId = UploadedFileMimeTypeCategoryIds.Video;
            else
                mimeTypeCategoryId = UploadedFileMimeTypeCategoryIds.Other;

            var file = new UploadedFile
            {
                CreatorAccountId = AccountId,
                FileId = fileId,
                TypeId = inputModel.FileTypeId.ToInt(),
                MimeType = inputModel.File.ContentType,
                MimeTypeCategoryId = mimeTypeCategoryId,
                Extension = Path.GetExtension(inputModel.File.FileName),
                Size = inputModel.File.Length,
                Name = inputModel.File.FileName
            };

            file = await _fileManager.SaveAsync(file, cancellationToken);
            return OkData(UploadedFileViewModel.Map(file));
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