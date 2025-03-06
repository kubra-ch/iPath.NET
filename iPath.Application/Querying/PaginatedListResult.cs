using iPath.Application.Features;

namespace iPath.Application.Querying;



public record PaginatedListResult<T> : BaseResponseT<IEnumerable<T>>
{
    public int TotalItemsCount { get; set; } = 0;
    public string? Messagae { get; set; }

    public PaginatedListResult(bool Success, string? Message = null) : base(Success, Message: Message)
    {
    }

    public PaginatedListResult(int TotalItemsCount, IEnumerable<T> Data) : base(Success: true, Data: Data)
    {
        this.TotalItemsCount = TotalItemsCount;
    }
}
