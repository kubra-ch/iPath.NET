using iPath.Application.Features;
using iPath.Data.Entities;

namespace iPath.UI.ViewModels.Nodes;

public class AnnotationModel
{
    public AnnotationModel(Annotation a)
    {
        Id = a.Id;
        Text = a.Text;
        CreatedOn = a.CreatedOn;
        if( a.Owner != null)
        {
            Owner = new UserListDto(a.Owner.Id, a.Owner.Username, a.Owner.Email);
        }
        else
        {
            Owner = new UserListDto(0, "--", "");
        }        
    }

    public readonly int Id;
    public readonly DateTime CreatedOn;
    public readonly string Text;
    public readonly UserListDto Owner;
}
