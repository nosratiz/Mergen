using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.Managers;
using Mergen.Game.Api.API.Chats.InputModels;
using Mergen.Game.Api.API.Chats.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Game.Api.API.Chats
{
    public class ChatController : ApiControllerBase
    {
        private readonly DataContext _context;
        private readonly AccountFriendManager _accountFriendManager;

        public ChatController(DataContext context, AccountFriendManager accountFriendManager)
        {
            _context = context;
            _accountFriendManager = accountFriendManager;
        }

        [HttpGet]
        [Route("chats/{accountId}/messages")]
        public async Task<ActionResult<IEnumerable<ChatViewModel>>> GetChatMessages([FromRoute]long accountId,
            long? afterId = null, long? beforeId = null, int count = 30, CancellationToken cancellationToken = default)
        {
            var messages = _context.Chats.AsNoTracking().Where(q =>
                (q.SenderAccountId == AccountId || q.SenderAccountId == accountId) &&
                (q.ReceiverAccountId == AccountId || q.ReceiverAccountId == accountId));

            if (afterId != null)
                messages = messages.Where(q => q.Id > afterId.Value).OrderBy(q => q.Id);

            if (beforeId != null)
                messages = messages.Where(q => q.Id < beforeId.Value).OrderByDescending(q => q.Id);

            messages = messages.Take(count);

            var result = await messages.ToListAsync(cancellationToken);

            return OkData(ChatViewModel.MapAll(result));
        }

        [HttpPost]
        [Route("chats/{accountId}/messages")]
        public async Task<ActionResult<ChatViewModel>> SendChatMessage([FromRoute] long accountId,
            [FromBody] ChatInputModel input, CancellationToken cancellationToken)
        {
            if (!await _accountFriendManager.IsFriendAsync(AccountId, accountId, cancellationToken))
                return Forbidden("not_friend", "This user is not a friend of you.");

            var chat = new Chat
            {
                SenderAccountId = AccountId,
                ReceiverAccountId = accountId,
                MessageText = input.Message,
                DateTime = DateTime.UtcNow,
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync(cancellationToken);

            return OkData(ChatViewModel.Map(chat));
        }
    }
}