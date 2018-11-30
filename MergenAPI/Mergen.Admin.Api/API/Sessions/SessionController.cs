using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Admin.Api.API.Sessions.InputViewModels;
using Mergen.Admin.Api.API.Sessions.ViewModels;
using Mergen.Admin.Api.Security.AuthenticationSystem;
using Mergen.Admin.Api.ViewModels;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Managers;
using Mergen.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Admin.Api.API.Sessions
{
    [ApiController]
    public class SessionController : ApiControllerBase
    {
        private readonly SessionManager _sessionManager;
        private readonly AccountManager _accountManager;
        private readonly JwtTokenGenerator _tokenGenerator;

        public SessionController(SessionManager sessionManager, AccountManager accountManager,
            JwtTokenGenerator tokenGenerator)
        {
            _sessionManager = sessionManager;
            _accountManager = accountManager;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        [Route("sessions")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResultViewModel<SessionViewModel>>> Login([FromBody] LoginInputModel model,
            CancellationToken cancellationToken)
        {
            var account = await _accountManager.FindByEmailAsync(model.Email, cancellationToken);
            if (account == null)
                return BadRequest("invalid_username_or_password", "Invalid Username or Password!");

            if (!PasswordHash.ValidatePassword(model.Password, account.PasswordHash))
                return BadRequest("invalid_username_or_password", "Invalid Username or Password!");

            var roles = await _accountManager.GetRolesAsync(account, cancellationToken);
            if (!roles.Contains(RoleIds.Admin))
            {
                return Forbidden();
            }

            if (!account.IsEmailVerified)
                return BadRequest("email_not_verified", "Please verify your email to log in.");

            var token = _tokenGenerator.GenerateToken(TimeSpan.FromDays(365),
                new Claim(JwtRegisteredClaimNames.Jti, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, account.Email));

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

        [HttpGet]
        [Route("sessions/{id}")]
        public async Task<ActionResult<ApiResultViewModel<SessionViewModel>>> GetSessionById(string id,
            CancellationToken cancellationToken)
        {
            var session = await _sessionManager.GetAsync(int.Parse(id), cancellationToken);
            if (session == null)
                return NotFound();

            return OkData(SessionViewModel.Map(session));
        }

        [HttpDelete]
        [Route("sessions/{id}")]
        public async Task<ActionResult> Logout(string id, CancellationToken cancellationToken)
        {
            var session = await _sessionManager.GetAsync(int.Parse(id), cancellationToken);
            if (session != null)
                await _sessionManager.DeleteAsync(session, cancellationToken);

            return Ok();
        }
    }
}