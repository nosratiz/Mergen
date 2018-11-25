using System;
using System.Linq;
using Mergen.Core.QueryProcessing;

namespace Mergen.Admin.Api.Security.AuthorizationSystem.PermissionParameterCheckers
{
    public class QueryPermissionChecker : IPermissionParameterChecker
    {
        private readonly string _queryFieldName;

        public string Key { get; }
        public QueryInputModel Value { get; }

        public bool CheckParameter(object[] values)
        {
            var fieldToCheck = Key;
            if (_queryFieldName != null)
                fieldToCheck = _queryFieldName;

            var parameter = Value.FilterParameters.FirstOrDefault(q =>
                string.Equals(q.FieldName, fieldToCheck, StringComparison.OrdinalIgnoreCase));

            if (parameter == null)
                return false;

            return parameter.Values.All(inputValue =>
                values.Any(permissionValue => string.Equals(permissionValue.ToString(), inputValue)));
        }

        public QueryPermissionChecker(string key, QueryInputModel value) : this(key, value, null)
        {
            Value = value;
        }

        public QueryPermissionChecker(string key, QueryInputModel value, string queryFieldName)
        {
            Key = key;
            Value = value;
            _queryFieldName = queryFieldName;
        }
    }
}