using System;
using System.Reflection;

namespace Mergen.Core.QueryProcessing
{
    public class FilterParameter
    {
        public PropertyInfo PropertyInfo;
        public string FieldName { get; set; }
        public Op Op { get; set; }
        public string[] Values { get; set; }

        public FilterParameter(string fieldName, Op op, string[] values, PropertyInfo propertyInfo)
        {
            if (values.Length == 0)
                throw new ArgumentException("no values provided");

            if (op == Op.Between && values.Length != 2)
                throw new ArgumentException("exactly two values required to use with between operator");

            if (op == Op.In && values.Length < 2)
                op = Op.Equals;

            FieldName = fieldName;
            Op = op;
            Values = values;
            PropertyInfo = propertyInfo;
        }

        public FilterParameter(string fieldName, Op op, string value, PropertyInfo propertyInfo) : this(fieldName, op,
            value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries), propertyInfo)
        {
        }
    }
}