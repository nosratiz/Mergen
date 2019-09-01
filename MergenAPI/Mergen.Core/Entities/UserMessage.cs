using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mergen.Core.Enums;

namespace Mergen.Core.Entities
{
    public class UserMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? SenderUserId { get; set; }

        public int ReceiverUserId { get; set; }

        public DateTime SendDate { get; set; }

        public DateTime? SeenDate { get; set; }

        public bool ReceiverHasDeleted { get; set; }

        public bool SenderHasDeleted { get; set; }

        public MessageType MessageType { get; set; }

        public int MessageId { get; set; }

        [ForeignKey("SenderUserId"), Column(Order = 0)]
        public virtual Account SenderUser { get; set; }

        [ForeignKey("ReceiverUserId"), Column(Order = 1)]
        public virtual Account ReceiverUser { get; set; }

        [ForeignKey(nameof(MessageId))]
        public virtual Message Message { get; set; }
    }
}