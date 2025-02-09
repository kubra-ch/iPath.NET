using iPath.Application.Features;
using iPath.Application.Services;
using iPath.Data.Entities;
using iPath.UI.Areas.Identity;
using Microsoft.AspNetCore.Components;
using System.Security.Cryptography.X509Certificates;

namespace iPath.UI.ViewModels.Nodes;

public class AnnotationModel
{
    public AnnotationModel(Annotation a)
    {
        Id = a.Id;
        OriginalText = a.Text!;
        LoadData(a);
    }

    public AnnotationModel(int NodeId, UserListDto Owner)
    {
        this.Id = 0;
        this.NodeId = NodeId;
        this.Owner = Owner;
    }

    public void LoadData(Annotation a)
    {
        if( Id != a.Id)  throw new Exception("can load data from another entity"); 

        Text = a.Text;
        Visibility = a.Visibility;
        CreatedOn = a.CreatedOn;
        ModifiedOn = a.ModifiedOn;
        Owner = a.Owner == null ? new UserListDto(0, "--", "") : new UserListDto(a.Owner.Id, a.Owner.Username, a.Owner.Email);
    }


    public readonly int Id;
    public readonly int NodeId;
    public DateTime CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }
    public string Text { get; set; }
    public readonly string OriginalText;
    public UserListDto Owner { get; private set; }
    public eAnnotationVisibility Visibility { get; private set; }
    public bool IsVisible => !(Visibility == eAnnotationVisibility.Deleted || Visibility == eAnnotationVisibility.Blaimed);


    public string CreatedOnStr => CreatedOn.ToShortDateString();
    public string ModifiedOnStr => ModifiedOn == null ? "" : ModifiedOn.Value.ToShortTimeString() + " " + ModifiedOn.Value.ToShortTimeString();


    public void ResetText()
    {
        Text = OriginalText;
    }

    public void SetDeleted()
    {
        Visibility = eAnnotationVisibility.Deleted;
    }

    public bool IsVisibleInSession(UserSession session)
    {
        if( session is null ) return false;
        if (Visibility == eAnnotationVisibility.Visible) return true;
        if (Owner.Id == session.UserId ) return true;
        return false;
    }

    public MarkupString Html => (MarkupString)StringConversionService.StringToHtml(this.Text);
}
