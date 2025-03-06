using iPath.Application.Querying;

namespace iPath.Application.Features;


public record NodeListResponse : PaginatedListResult<NodeListDTO>
{
    public NodeListResponse(bool Success, string? Message = null)
        : base(Success, Message: Message) { }

    public NodeListResponse(int TotalItemsCount, IEnumerable<NodeListDTO> Data)
        : base(TotalItemsCount: TotalItemsCount, Data: Data) { }
}