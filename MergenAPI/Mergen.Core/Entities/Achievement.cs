using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class Achievement : Entity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageFileId { get; set; }
    }
}