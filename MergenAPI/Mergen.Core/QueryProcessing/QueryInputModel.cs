using System;

namespace Mergen.Core.QueryProcessing
{
    public class QueryInputModel
    {
        public FilterParameter[] FilterParameters { get; set; }
        public SortParameter[] SortParameters { get; set; }
        public PaginationParameter PaginationParameter { get; set; }
    }

    public class QueryInputModel<TEntity> : QueryInputModel
    {
        public Type ModelType { get; } = typeof(TEntity);
    }
}