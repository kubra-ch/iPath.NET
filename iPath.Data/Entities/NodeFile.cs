namespace iPath.Data.Entities;

public class NodeFile : BaseEntity
{
    public int NodeId { get; set; }

    public string Filename { get; set; } = default!;
    public string? Originalname { get; set; } = default!;
    public string? MimeType { get; set; }
    public int? Filesize { get; set; }

    public bool IsImage { get; set; }
    public string? ThumbData { get; set; }
    public int? ImageWidth { get; set; }
    public int? ImageHeight { get; set; }
}
