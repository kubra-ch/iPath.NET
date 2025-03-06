using iPath.Data.Entities;

namespace iPath.Application.Features;

public record NodeCommandResponse(bool Success, string? Message = default!, Node? node = null!)
    : BaseResponseT<Node>(Success, Message, node);
