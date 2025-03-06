using System.ComponentModel;

namespace iPath.Application.Querying;


public class PaginatedListQuery
{
    [DefaultValue(0)]
    public int Page { get; set; } = 0;

    [DefaultValue(10)]
    public int? PageSize { get; set; } = 10;

    [DefaultValue(null)]
    public RootFilter Filter { get; set; } = new();

    [DefaultValue(null)]
    public List<SortDefinition>? SortDefinitions { get; set; } = new();
}


public static class PaginatedListQueryExtension
{
    public static PaginatedListQuery AddSorting(this PaginatedListQuery q, string SortBy, bool Ascending)
    {
        q.SortDefinitions ??= new();
        q.SortDefinitions.Add(new SortDefinition(SortBy, Ascending));
        return q;
    }

    public static PaginatedListQuery AddFilter(this PaginatedListQuery q, string Field, object Value)
    {
        q.Filter ??= new();
        q.Filter.AddFilter(Field, Value);
        return q;
    }

    public static string GetFilterValue(this PaginatedListQuery q, string Field)
    {
        if (q.Filter is null || q.Filter.Filters is null || !q.Filter.Filters.Any()) return "";
        foreach (var item in q.Filter.Filters)
        {
            if (item.Value != null && string.Compare(item.Field, Field, true) == 0)
            {
                return item.Value.ToString();
            }
        }
        return "";
    }
}