using System.ComponentModel.DataAnnotations;

namespace LearnHub.Models;

/// <summary>Links a registered member to a resource they are studying.</summary>
public class Enrollment
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public int ResourceId { get; set; }
    public LearningResource? Resource { get; set; }

    [Range(0, 100)]
    [Display(Name = "Progress (%)")]
    public int ProgressPercent { get; set; }

    [Display(Name = "Status")]
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.InProgress;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

public enum EnrollmentStatus
{
    InProgress = 0,
    Completed = 1
}
