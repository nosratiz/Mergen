using System.Collections.Generic;
using System.Linq;

namespace Mergen.Admin.Api.Security.AuthorizationSystem.PermissionParameterCheckers
{
    public abstract class PermissionParameterChecker<T> : IPermissionParameterChecker
    {
        private readonly T _value;
        public string Key { get; }
        public virtual IEqualityComparer<T> Comparer { get; } = EqualityComparer<T>.Default;

        public PermissionParameterChecker(string key, T value)
        {
            _value = value;
            Key = key;
        }

        public virtual bool Equals(T v1, T v2)
        {
            return Comparer.Equals(v1, v2);
        }

        public virtual T Convert(object value)
        {
            return (T) System.Convert.ChangeType(value, typeof(T));
        }

        public virtual bool CheckParameter(object[] values)
        {
            return values.Any(v => Equals(Convert(v), Convert(_value)));
        }
    }
}