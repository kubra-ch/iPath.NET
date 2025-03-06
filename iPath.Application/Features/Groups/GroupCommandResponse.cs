using iPath.Data.Entities;

namespace iPath.Application.Features;

public record GroupCommandResponse(bool Success, string? Message = default!, Group? group = null!)
    : BaseResponseT<Group>(Success, Message, group);
