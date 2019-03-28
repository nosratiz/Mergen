namespace Mergen.Api.Core.ViewModels
{
    public class DataMetaViewModel
    {
        public int? TotalCount { get; set; }

        public DataMetaViewModel(int? totalCount)
        {
            TotalCount = totalCount;
        }
    }
}