namespace Mergen.Game.Api.Security.AuthorizationSystem
{
    public interface IPermissionParameterChecker
    {
        string Key { get; }

        bool CheckParameter(object[] values);
    }
}