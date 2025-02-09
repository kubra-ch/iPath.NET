using iPath.Application.Features;
using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Identity.Client;

namespace iPath.UI.ViewModels.Drafts;

public interface IDraft
{
    string DraftId { get; set; }
    DateTime CreatedOn { get; set; }
}


