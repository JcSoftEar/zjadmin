using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZJAdmin.Api.Models;

[Table("sys_operation_log")]
public class OperationLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [MaxLength(50)]
    public string? Operator { get; set; }

    [MaxLength(50)]
    public string? Module { get; set; }

    [MaxLength(20)]
    public string? OperationType { get; set; }

    [MaxLength(500)]
    public string? RequestUrl { get; set; }

    [MaxLength(10)]
    public string? RequestMethod { get; set; }

    public string? RequestParams { get; set; }

    public string? ResponseResult { get; set; }

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    public byte Status { get; set; } // 0-失败, 1-成功

    public long Duration { get; set; }

    public DateTime OperationTime { get; set; } = DateTime.UtcNow;
}
