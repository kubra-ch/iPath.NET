﻿using iPath.Application.Configuration;
using iPath.Application.Features;
using iPath.Application.Services;
using iPath.Data.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace iPath.UI.ViewModels;

public class NodeModel
{
    public NodeModel()
    {   
    }

    public NodeModel(Node node, GroupDto group)
    {
        Id = node.Id;
        Group = group;
        Title = node.Title;
        NodeType = node.NodeType.Name.ToLower();
        Description = node.Description;
        CreatedOn = node.CreatedOn;
        SubTitle = node.SubTitle;
        SortNr = node.SortNr;
        Status = node.Status;
        Visibility = node.Visibility;

        if (node.Owner != null)
        {
            Owner = node.Owner.ToListDto();
        }

        if ( node.File != null)
        {
            Filename = node.File.Filename;
            MimeType = node.File.MimeType;
            ThumbData = node.File.ThumbData;
            IsImage = node.File.IsImage;
            ImageWidth = node.File.ImageWidth;
            ImageHeight = node.File.ImageHeight;
        }


        if (node.ChildNodes != null && node.ChildNodes.Any())
        {
            foreach (var child in node.ChildNodes)
            {
                this.Children.Add(new NodeModel(child, group));
            }
        }

        if (node.Annotations != null && node.Annotations.Any())
        {
            foreach (var annotation in node.Annotations)
            {
                this.Annotations.Add(new AnnotationModel(annotation));
            }
        }
    }


    public int Id { get; private set; }
    public GroupDto? Group { get; private set; }

    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string NodeType { get; set; }
    public string Description { get; set; }
    public DateTime CreatedOn { get; private set; }
    public int? SortNr { get; set; }
    public int? NewSortNr { get; set; } // placeholder for re-ordering

    public eNodeStatus Status { get; set; }
    public eNodeVisibility Visibility { get; set; }

    public string Filename { get; set; }
    public string MimeType { get; private set; }
    public string ThumbData { get; private set; }
    public bool IsImage { get; private set; }
    public int? ImageWidth { get; private set; }
    public int? ImageHeight { get; private set; }

    public UserListDto Owner { get; set; }


    public List<NodeModel> Children { get; private set; } = new();
    public List<NodeModel> VisibleChildren => Children.Where(c => c.Visibility == eNodeVisibility.Visible || c.Visibility == eNodeVisibility.Public)
            .OrderBy(c => c.SortNr).ToList();

    public bool HasUnfinishedUploads(int? UserId) => Children.Any(c => c.Visibility == eNodeVisibility.Draft && (!UserId.HasValue || c.Owner.Id == UserId));
    public bool HasDeletedNodes => Children.Any(c => c.Visibility == eNodeVisibility.Deleted);


    public List<AnnotationModel> Annotations { get; private set; } = new();
    public void AddAnnotation(Annotation entity)
    {
        Annotations.RemoveAll(a => a.Id == entity.Id);
        Annotations.Add(new AnnotationModel(entity));
    }


    public string Caption => !string.IsNullOrEmpty(Title) ? Title : Filename;

    public MarkupString DescriptionHtml
    {
        get => (MarkupString)StringConversionService.StringToHtml(this.Description);
        set => this.Description = value.Value;
    }


    [FromServices]
    public IOptions<iPathConfig> Opts { get; set; }

    public NodeModel Parent { get; set; }

    public string CreatedDateStr => CreatedOn.ToShortDateString();
    public string OwnerName => Owner != null ? Owner.Username : "";

    public bool HasSubTitle => !string.IsNullOrWhiteSpace(SubTitle);


    public string ImageCaption => $"{SortNr} - " + Filename?.Substring(0, Math.Min(12, Filename.Length - 1));


    public bool HasAnnotationDraft(int Userid) => Annotations.Any(a => a.Owner.Id == Userid && a.Visibility == eAnnotationVisibility.Draft);
}

