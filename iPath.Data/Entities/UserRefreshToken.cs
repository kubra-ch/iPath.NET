using System.ComponentModel.DataAnnotations;

namespace iPath.Data.Entities;

public class UserRefreshToken : BaseEntity
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiredAt { get; set; }
}