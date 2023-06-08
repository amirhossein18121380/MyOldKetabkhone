namespace DataModel.Common
{
    public class FileSystem
    {
        public long Id { get; set; }
        public string FileName { get; set; } = null!;
        public long FileSize { get; set; }
        public string FileType { get; set; } = null!;
        public byte[] FileData { get; set; } = null!;
        public string ThumbnailFileName { get; set; } = null!;
        public long ThumbnailFileSize { get; set; }
        public byte[] ThumbnailFileData { get; set; } = null!;
        public bool IsCompress { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreateOn { get; set; }
    }
}