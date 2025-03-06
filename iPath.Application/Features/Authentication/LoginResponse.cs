using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Areas.Authentication;

public record LoginResponse(
    bool Success, 
    string? AccessToken = null!, 
    string? RefreshToken = null!, 
    string? Message=null!);
