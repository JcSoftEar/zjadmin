using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZJAdmin.Api.Models;

[Table("sys_user")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Nickname { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    public byte Status { get; set; } = 1;

    public byte IsDeleted { get; set; } = 0;

    public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateTime { get; set; }

    public int LoginFailCount { get; set; } = 0;

    public DateTime? LockoutEndTime { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
