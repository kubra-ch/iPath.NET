using iPath.Data.Entities;

namespace iPath.Application.Features;

public record UserCommandResponse(bool Success, string? Message = default!, User? Data = null!)
    : BaseResponseT<User>(Success, Message, Data);

