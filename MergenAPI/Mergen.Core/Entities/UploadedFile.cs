using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class UploadedFile : Entity
    {
        public string FileId { get; set; }
        public long CreatorAccountId { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }
        public long? Size { get; set; }
        public string MimeType { get; set; }
        public int? MimeTypeCategoryId { get; set; }
        public string Extension { get; set; }
    }
}