namespace Mergen.Core.Options
{
    public class FileOptions
    {
        public string BaseStoragePath { get; set; }

        public string AvatarsFolderName { get; set; }

        public string FilesFolderName { get; set; }

        public string ProjectLogosFolderName { get; set; }

        public string ProjectCoverImagesFolderName { get; set; }

        public string ChannelLogosFolderName { get; set; }

        public string EventsFolderName { get; set; }

        public long MaximumFileSize { get; set; }

        public long MaximumAvatarSize { get; set; }

        public long MaximumProjectLogoSize { get; set; }

        public long MaximumProjectCoverImageSize { get; set; }

        public long MaximumChannelLogoSize { get; set; }

        public long MaximumEventSize { get; set; }
    }
}