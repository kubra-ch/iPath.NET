using System.ComponentModel;

namespace iPath.Application.Querying;


public class PaginatedListQuery
{
    [DefaultValue(0)]
    public int StartIndex { get; set; } = 0;

    [DefaultValue(10)]
    public int? Count { get; set; } = 10;


    [DefaultValue(null)]
    public RootFilter Filter { get; set; } = new();

    [DefaultValue(null)]
    public List<SortDefinition>? SortDefinitions { get; set; } = new();
}
