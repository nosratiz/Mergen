using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Managers;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Game.Api.API.Notification
{
    public class NotificationController : ApiControllerBase
    {
        private readonly NotificationManager _notificationManager;
        private readonly AccountManager _accountManager;

        public NotificationController(NotificationManager notificationManager, AccountManager accountManager)
        {
            _notificationManager = notificationManager;
            _accountManager = accountManager;
        }

        [HttpGet]
        [Route("Notification/{accountId}")]
        public async Task<IActionResult> GetNotification([FromRoute]long accountId, CancellationToken cancellationToken)
        {
            if (AccountId != accountId)
                return Forbidden();

            var notification = await _notificationManager.GetNotificationListAsync(accountId, cancellationToken);

            return OkData(notification);
        }
    }
}