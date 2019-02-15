using Mergen.Core.Entities.Base;
using Mergen.Core.EntityIds;

namespace Mergen.Core.Entities
{
    public class Category : Entity
    {
        public string Title { get; set; }
        public CategoryStatusIds StatusId { get; set; }
        public string IconFileId { get; set; }
        public string CoverImageFileId { get; set; }
        public string Description { get; set; }
    }
}