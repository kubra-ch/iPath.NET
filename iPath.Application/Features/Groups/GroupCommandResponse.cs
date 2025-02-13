using iPath.Data.Entities;

namespace iPath.Application.Features;

public record GroupCommandResponse(bool Success, string? Message = default!, GroupDto? Data = null!)
    : BaseResponseT<GroupDto>(Success, Message, Data);
