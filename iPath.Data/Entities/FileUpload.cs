using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPath.Data.Entities;

public class FileUpload : BaseEntity
{
    public string TempFile { get; set; }
    public string Filename { get; set; }
    public string MimeType { get; set; }

    public int OwnerId { get; set; }
    public User Owner { get; set; }

    public int NodeId { get; set; }
    public Node Node { get; set; }
    public int ParentId { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
