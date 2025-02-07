using iPath.Data.Entities;

namespace iPath.Application.Features;

public record NodeCommandRespone(bool Success, string? Message = default!, Node? Data = null!)
    : BaseResponseT<Node>(Success, Message, Data);
