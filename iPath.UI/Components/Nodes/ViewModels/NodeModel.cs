using iPath.Application.Features;
using iPath.Data.Entities;
using iPath.Application.Services.Cache;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace iPath.UI.Components.Nodes.ViewModels;

public class NodeModel
{
    public int Id { get; private set; }
    public string NodeType { get; private set; }
    public int? RootNodeId { get; private set; }
    public int? ParentId { get; private set; }

    public DateTime CreatedOn { get; private set; }

    public UserDTO Owner { get; private set; }
    public GroupDTO  Group { get; private set; }

    public NodeDescription Description { get; private set; } = new();
    public NodeFile File { get; private set; } = new();
    public int? SortNr { get; set; }

    public List<NodeModel> ChildNodes { get; private set; } = new();
    public List<AnnotationModel> Annotations { get; private set; } = new();



    public bool IsImage => (NodeType == "image");

    public string FileIcon
    {
        get
        {
            if (File != null && File.MimeType.ToLower().EndsWith("pdf"))
                return MudBlazor.Icons.Custom.FileFormats.FilePdf;

            if( NodeType.ToLower() == "folder")
                return MudBlazor.Icons.Material.Filled.FolderOpen;

            return MudBlazor.Icons.Custom.FileFormats.FileDocument;
        }
    }



    public MarkupString DescriptionHtml
    {
        get 
        {
            var html = Description.Text ?? "";

            // replace line breaks
            html = html.ReplaceLineEndings("<br />\n");

            // replace links
            html = MakeLink(html);

            return (MarkupString) html;
        }
    }

    protected string MakeLink(string txt)
    {
        txt = Regex.Replace(txt,
                @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)",
                "<a target='_blank' href='$1'>$1</a>");
        return txt;
    }



    private NodeModel()
    {
    }


    public static NodeModel CreateModel()
    {
        return new NodeModel();
    }

    public static async Task<NodeModel> CreateModelAsync(Node node, IDataCache cache)
    {
        var model = new NodeModel
        {
            Id = node.Id,
            NodeType = node.NodeType,
            RootNodeId = node.RootNodeId,
            ParentId = node.ParentNodeId,
            CreatedOn = node.CreatedOn,
            Description = node.Description,
            File = node.File,
            SortNr = node.SortNr,
        };

        var profile = await cache.GetProfileAsync(node.OwnerId);
        model.Owner = new UserDTO { UserId = profile.UserId, Username = profile.Username };
        model.Group = node.GroupId.HasValue ? (await cache.GetGroupDtoAsync(node.GroupId.Value)) : null!;

        if (node.Annotations != null && node.Annotations.Any())
        {
            foreach (var annotation in node.Annotations)
            {
                model.Annotations.Add(await AnnotationModel.CreateModelAsync(annotation, cache));
            }
        }

        if (node.ChildNodes != null && node.ChildNodes.Any())
        {
            foreach( var child in node.ChildNodes.Where(c => c.ParentNodeId == node.Id).OrderBy(c => c.SortNr))
            {
                model.ChildNodes.Add(await CreateModelAsync(child, cache));
            }
        }

        return model;
    }



    public NodeModel GetSibling(NodeModel child, bool next)
    {
        // next image inside Node
        var ids = this.ChildNodes.Where(c => c.ParentId == child.ParentId).OrderBy(c => c.SortNr).Select(c => c.Id).ToList();
        var idx = ids.IndexOf(child.Id);
        if (next && idx < ids.Count() - 1)
        {
            return this.ChildNodes.FirstOrDefault(c => c.Id == ids[idx + 1]);
        }
        else if ( !next && idx > 0)
        {
            return this.ChildNodes.FirstOrDefault(c => c.Id == ids[idx - 1]);
        }
        else if( child.ParentId.HasValue )
        {
            // go up
            return this.GetParent(child);
        }
        else
        {
            // back to self
            return this;
        }
    }

    public NodeModel GetParent(NodeModel child)
    {
        if( child.ParentId.HasValue )
        {
            if ( child.ParentId.Value == this.Id )
            {
                return this;
            }
            else
            {
                return this.ChildNodes.FirstOrDefault(c => c.ParentId == child.ParentId.Value);  
            }
        }
        return null;
    }


    public string GalleryCaption => File is null ? "" : File.Filename;


}
