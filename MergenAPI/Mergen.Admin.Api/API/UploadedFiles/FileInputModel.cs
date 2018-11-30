using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Mergen.Admin.Api.API.UploadedFiles
{
    public class FileInputModel
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public string FileTypeId { get; set; }
    }
}