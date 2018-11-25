using System.Collections.Generic;

namespace Mergen.Core.QueryProcessing
{
    public class QueryResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}