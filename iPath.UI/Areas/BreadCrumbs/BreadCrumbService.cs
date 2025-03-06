using Microsoft.Extensions.Localization;
using MudBlazor;
using System.ComponentModel;

namespace iPath.UI.Areas.BreadCrumbs;

public class BreadCrumbService(IStringLocalizer T)
    : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;


    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
       

    public List<BreadcrumbItem> Breadcrumbs { get; private set; }


    public void LoadItems(BreadCrumb[] items)
    {
        var tmp = new List<BreadcrumbItem>();
        if (items != null)
        {
            foreach (var item in items)
            {
                tmp.Add(new BreadcrumbItem(text: T[item.Text], href: item.href, disabled: string.IsNullOrEmpty(item.href)));
            }
        }
        Breadcrumbs = tmp;
        OnPropertyChanged(nameof(Breadcrumbs));
    }

    public bool HasBreadCrumbs => Breadcrumbs != null && Breadcrumbs.Any();
}
