namespace Mergen.Api.Core.Security.AuthorizationSystem
{
    public class PermissionParameter
    {
        public string Key { get; set; }
        public object[] Values { get; set; }

        public PermissionParameter(string key, params object[] value)
        {
            Key = key;
            Values = value;
        }
    }
}