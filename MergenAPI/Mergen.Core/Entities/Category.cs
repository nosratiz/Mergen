using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class Category : Entity
    {
        public string Title { get; set; }
        public long StatusId { get; set; }
        public string IconFileId { get; set; }
        public string CoverImageFileId { get; set; }
        public string Description { get; set; }
    }
}