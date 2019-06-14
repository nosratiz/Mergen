using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Managers;
using Mergen.Core.Security;
using Mergen.Game.Api.Security.AuthenticationSystem;
using Mergen.Game.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Mergen.Game.Api.API.Accounts
{
    public class SessionController : ApiControllerBase
    {
        private readonly AccountManager _accountManager;
        private readonly SessionManager _sessionManager;
        private readonly JwtTokenGenerator _tokenGenerator;

        public SessionController(AccountManager accountManager, SessionManager sessionManager, JwtTokenGenerator tokenGenerator)
        {
            _accountManager = accountManager;
            _sessionManager = sessionManager;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        [Route("sessions")]
        public async Task<ActionResult<ApiResultViewModel<SessionViewModel>>> Login([FromBody] LoginInputModel inputModel,
            CancellationToken cancellationToken)
        {
            var account = await _accountManager.FindByEmailAsync(inputModel.Email, cancellationToken);
            if (account == null || account.IsArchived)
                return BadRequest("invalid_email", "Account not found");

            if (!PasswordHash.ValidatePassword(inputModel.Password, account.PasswordHash))
                return BadRequest("invalid_username_or_password", "Invalid Nickname or Password!");

            var token = _tokenGenerator.GenerateToken(TimeSpan.FromDays(365),
                new Claim(JwtRegisteredClaimNames.Jti, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, account.Email),
                new Claim("Timezone", account.Timezone));

            var session = new Session
            {
                AccessToken = token,
                AccountId = account.Id,
                CreationDateTime = DateTime.UtcNow,
                StateId = SessionStateIds.Created
            };

            await _sessionManager.SaveAsync(session, cancellationToken);

            return CreatedData(SessionViewModel.Map(session));
        }
    }
}