using iPath.Data.Entities;

namespace iPath.Application.Features;

public record CommunityCommandResponse(bool Success, string? Message = default!, Community? community = null!)
    : BaseResponseT<Community>(Success, Message, community);
