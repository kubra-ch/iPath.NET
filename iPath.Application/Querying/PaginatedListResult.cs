namespace iPath.Application.Querying;




public class PaginatedListResult<T>
{
    public int TotalItemsCount { get; set; } = 0;
    public List<T> Items { get; set; } = new();

    public PaginatedListResult()
    {
    }

    public PaginatedListResult(List<T> items, int total)
    {
        Items = items;
        TotalItemsCount = total;
    }
}
