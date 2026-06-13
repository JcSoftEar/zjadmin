using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZJAdmin.Api.Models;

[Table("sys_exception_log")]
public class ExceptionLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string? Message { get; set; }

    [MaxLength(200)]
    public string? ExceptionType { get; set; }

    public string? StackTrace { get; set; }

    [MaxLength(500)]
    public string? RequestUrl { get; set; }

    [MaxLength(10)]
    public string? RequestMethod { get; set; }

    public string? RequestParams { get; set; }

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    [MaxLength(50)]
    public string? Operator { get; set; }

    public DateTime OccurTime { get; set; } = DateTime.UtcNow;
}
