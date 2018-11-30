using Mergen.Core.Entities;

namespace Mergen.Admin.Api.API.UploadedFiles
{
    public class UploadedFileViewModel
    {
        public string FileId { get; set; }
        public string Name { get; set; }
        public string FileTypeId { get; set; }
        public string MimeType { get; set; }

        public static UploadedFileViewModel Map(UploadedFile file)
        {
            return new UploadedFileViewModel
            {
                FileId = file.FileId,
                Name = file.Name,
                FileTypeId = file.TypeId.ToString(),
                MimeType = file.MimeType
            };
        }
    }
}