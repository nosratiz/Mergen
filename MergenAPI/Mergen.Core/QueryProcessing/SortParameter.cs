namespace Mergen.Core.QueryProcessing
{
    public class SortParameter
    {
        public string Field { get; set; }
        public bool Desc { get; set; }

        public SortParameter(string field, bool desc = false)
        {
            Field = field;
            Desc = desc;
        }
    }
}