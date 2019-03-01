using Microsoft.AspNetCore.Http;

namespace Mergen.Admin.Api.API.Questions
{
    public class QuestionFileImportInputModel
    {
        public string CategoryId { get; set; }
        public IFormFile File { get; set; }
    }
}