using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mergen.Core.Entities.Base
{
    public class Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public bool IsArchived { get; set; }
    }
}