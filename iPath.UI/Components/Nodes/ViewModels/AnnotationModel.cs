using iPath.Application.Features;
using iPath.Data.Entities;
using iPath.Application.Services.Cache;

namespace iPath.UI.Components.Nodes.ViewModels;

public class AnnotationModel
{
    public int Id { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public string Text { get; private set; }
    public UserDTO Owner { get; private set; }


    private AnnotationModel()
    {   
    }


    public static async Task<AnnotationModel> CreateModelAsync(Annotation a, IDataCache cache)
    {
        var model = new AnnotationModel();
        model.Id = a.Id;
        model.CreatedOn = a.CreatedOn;
        model.Text = a.Text;
        model.Owner = (await cache.GetProfileAsync(a.OwnerId)).ToOwner();
        return model;
    }
}
