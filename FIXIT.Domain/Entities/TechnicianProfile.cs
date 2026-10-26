using System.ComponentModel.DataAnnotations;
namespace FIXIT.Domain.Entities;
public class TechnicianProfile
{
    [Key] public string UserId { get; set; } = "";
    public User User { get; set; } = null!;
    [Required, StringLength(1000)] public string Skills { get; set; } = "";
    [Required, StringLength(300)] public string CoverageAreas { get; set; } = "";
    public TechnicianStatus Status { get; set; } = TechnicianStatus.Pending;
    public bool IsOnline { get; set; }
    public string? DecisionReason { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public ICollection<TechnicianSkill> TechnicianSkills { get; set; } = [];
    public ICollection<TechnicianCoverageArea> TechnicianCoverageAreas { get; set; } = [];
}
public enum TechnicianStatus { Pending, Approved, Rejected, Suspended }
