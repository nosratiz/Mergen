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

        [HttpDelete]
        [Route("Notification/{id}")]
        public async Task<IActionResult> DeleteNotification([FromRoute] long id, CancellationToken cancellationToken)
        {
            var notification = await _notificationManager.GetAsync(id, cancellationToken);

            if (notification is null)
                return NotFound("Notification Not Found", "the notification you want to delete was not found");

            await _notificationManager.DeleteAsync(notification, cancellationToken);

            return NoContent();

        }
    }
}