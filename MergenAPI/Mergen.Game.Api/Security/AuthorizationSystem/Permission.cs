using System.Collections.Generic;

namespace Mergen.Game.Api.Security.AuthorizationSystem
{
    public class Permission
    {
        public string Key { get; set; }

        private readonly List<PermissionParameter> _parameters;

        public IEnumerable<PermissionParameter> Parameters => _parameters;

        public Permission(string key)
        {
            Key = key;
            _parameters = new List<PermissionParameter>();
        }

        public Permission AddParameter(PermissionParameter param)
        {
            _parameters.Add(param);
            return this;
        }

        public Permission AddParameter(string key, params object[] values)
        {
            _parameters.Add(new PermissionParameter(key, values));
            return this;
        }
    }
}