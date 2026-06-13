using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZJAdmin.Api.Models;

[Table("sys_permission")]
public class Permission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long ParentId { get; set; } = 0;

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    public byte Type { get; set; } // 0-菜单, 1-按钮

    [MaxLength(200)]
    public string? Path { get; set; }

    [MaxLength(200)]
    public string? Component { get; set; }

    [MaxLength(50)]
    public string? Icon { get; set; }

    public int Sort { get; set; } = 0;

    public byte Visible { get; set; } = 1;

    public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateTime { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
