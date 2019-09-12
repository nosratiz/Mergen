namespace Mergen.Game.Api.API.Accounts.ViewModels
{
    public class RegisterViewModel
    {
        public AccountViewModel AccountViewModel { get; set; }

        public SessionViewModel SessionViewModel { get; set; }

        public static RegisterViewModel GetRegisterViewModel(AccountViewModel accountViewModel,
            SessionViewModel sessionViewModel)
        {
            return new RegisterViewModel
            {
                AccountViewModel = accountViewModel,
                SessionViewModel = sessionViewModel

            };
        }
    }
}