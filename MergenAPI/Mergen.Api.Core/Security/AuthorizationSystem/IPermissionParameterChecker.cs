namespace Mergen.Api.Core.Security.AuthorizationSystem
{
    public interface IPermissionParameterChecker
    {
        string Key { get; }

        bool CheckParameter(object[] values);
    }
}