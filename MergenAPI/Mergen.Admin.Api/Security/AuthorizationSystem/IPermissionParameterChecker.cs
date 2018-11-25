namespace Mergen.Admin.Api.Security.AuthorizationSystem
{
    public interface IPermissionParameterChecker
    {
        string Key { get; }

        bool CheckParameter(object[] values);
    }
}