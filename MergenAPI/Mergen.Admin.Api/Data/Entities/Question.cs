namespace Mergen.Admin.Api.Data.Entities
{
	public class Question
	{
		public string Body { get; set; }
		public int Difficulty { get; set; }
		public string Answer1 { get; set; }
		public string Answer2 { get; set; }
		public string Answer3 { get; set; }
		public string Answer4 { get; set; }
		public int CorrectAnswerId { get; set; }
	}
}