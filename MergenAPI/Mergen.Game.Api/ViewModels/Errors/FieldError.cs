using Mergen.Game.Api.ViewModels.Errors;

namespace MMergen.Game.ApiViewModels.Errors
{
    public class FieldError : ErrorViewModel
    {
        public string FieldName { get; set; }
    }
}