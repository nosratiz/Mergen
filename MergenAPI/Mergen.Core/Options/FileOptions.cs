namespace Mergen.Core.Options
{
    public class FileOptions
    {
        public string BaseStoragePath { get; set; }

        public long MaximumFileSize { get; set; }

        public long MaximumAvatarSize { get; set; }

        public long MaximumProjectLogoSize { get; set; }

        public long MaximumProjectCoverImageSize { get; set; }

        public long MaximumChannelLogoSize { get; set; }

        public long MaximumEventSize { get; set; }
    }
}