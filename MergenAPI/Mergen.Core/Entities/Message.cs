using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mergen.Core.Entities
{
    public class Message
    {
        public Message()
        {
            UserMessages = new List<UserMessage>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime CreateDate { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual Message ReplayToMessage { get; set; }

        public virtual List<UserMessage> UserMessages { get; }
    }
}