using Ardalis.SmartEnum;

namespace iPath.Data.Entities;

/*
public class NodeType : BaseEntity
{
    public string Name { get; set; }
    public bool AcceptChildNodes { get; set; }
}
*/

public sealed class NodeType : SmartEnum<NodeType>
{
    public static readonly NodeType None = new("None", 0);
    public static readonly NodeType Case = new("Case", 1);
    public static readonly NodeType Folder = new("Folder", 2);
    public static readonly NodeType Image = new("Image", 3);
    public static readonly NodeType File = new("File", 4);
    public static readonly NodeType Link = new("Link", 5);
    public static readonly NodeType Textslide = new("Textslide", 6);
    public static readonly NodeType Video = new("Video", 7);
    public static readonly NodeType Dicom = new("Dicom", 8);
    public static readonly NodeType WSI = new("WSI", 9);
    private NodeType(string name, int id) : base(name, id) { }
}