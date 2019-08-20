using System;
using System.Collections.Generic;
using System.Linq;
using Mergen.Core.Entities;

namespace Mergen.Game.Api.API.Chats.ViewModels
{
    public class ChatViewModel : EntityViewModel
    {
        public string SenderAccountId { get; set; }
        public string ReceiverAccountId { get; set; }
        public string MessageText { get; set; }
        public DateTime DateTime { get; set; }

        public static IEnumerable<ChatViewModel> MapAll(IEnumerable<Chat> chats)
        {
            return chats.Select(Map);
        }

        public static ChatViewModel Map(Chat chat)
        {
            return new ChatViewModel
            {
                Id = chat.Id,
                SenderAccountId = chat.SenderAccountId.ToString(),
                ReceiverAccountId = chat.ReceiverAccountId.ToString(),
                DateTime = chat.DateTime,
                MessageText = chat.MessageText
            };
        }
    }
}