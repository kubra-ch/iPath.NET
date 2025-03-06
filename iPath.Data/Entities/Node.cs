using System.Text.Json.Serialization;

namespace iPath.Data.Entities;

public class Node : BaseEntityWithDeleteFlag
{
    public string? StorageId { get; set; }

    public DateTime CreatedOn { get; set; }
    public int OwnerId { get; set; }
    public User Owner { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public int? GroupId { get; set; }
    public Group? Group { get; set; }

    [JsonIgnore]
    public int? RootNodeId { get; set; }

    [JsonIgnore]
    public Node? RootNode { get; set; }

    [JsonIgnore]
    public int? ParentNodeId { get; set; }

    [JsonIgnore]
    public Node? ParentNode { get; set; }

    [JsonIgnore]
    public NodeImport? ImportedData { get; set; }

    public int? SortNr { get; set; } = 0;


    public ICollection<Node> ChildNodes { get; set; } = [];
    public ICollection<Annotation> Annotations { get; set; } = [];


    public ICollection<FileUpload> Uploads { get; set; } = [];
    public ICollection<NodeLastVisit> Visits { get; set; }

    public string NodeType { get; set; } = default!;

    public NodeDescription Description { get; set; } = new();

    public NodeFile? File { get; set; } = null!;
}

public class NodeImport
{
    public int NodeId { get; set; }
    public string? Data { get; set; }
    public string? Info { get; set; }
}



public class NodeDescription
{
    public string? Subtitle { get; set; }
    public string? CaseType { get; set; }
    public string? AccessionNo { get; set; }
    public string? Status { get; set; }
    public string Title { get; set; } = default!;
    public string Text { get; set; } = default!;
}

public class NodeFile
{
    public DateTime? LastStorageExportDate { get; set; }
    public string? Filename { get; set; }
    public string? MimeType { get; set; }
    public string? ThumbData { get; set; }
    public int? ImageWidth { get; set; }
    public int? ImageHeight { get; set; }
}



public class UserDTO
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Initials { get; set; }

    public override string ToString()
    {
        return Username;
    }
}




public class CodeableConcept
{
    public string System { get; set; }
    public string Code { get; set; }
    public string Display { get; set; }

    public override string ToString()
    {
        return Display;
    }
}

