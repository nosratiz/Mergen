namespace Mergen.Core.QueryProcessing
{
    public class PaginationParameter
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public PaginationParameter(int pageSize, int pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }
    }
}