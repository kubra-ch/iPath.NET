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
    public static readonly NodeType Case = new("Case", 1);
    public static readonly NodeType Folder = new("Folder", 2);
    public static readonly NodeType Image = new("Image", 3);
    private NodeType(string name, int id) : base(name, id) { }
}