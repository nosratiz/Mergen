using System.ComponentModel.DataAnnotations;

namespace Mergen.Admin.Api.API.Questions
{
    public class QuestionInputModel
    {
        [Required]
        public string Body { get; set; }
        [Required]
        public int Difficulty { get; set; }
        [Required]
        public string Answer1 { get; set; }
        [Required]
        public string Answer2 { get; set; }
        [Required]
        public string Answer3 { get; set; }
        [Required]
        public string Answer4 { get; set; }
        [Required]
        public int CorrectAnswerNumber { get; set; }
        public string[] CategoryIds { get; set; }
    }
}