using System.ComponentModel.DataAnnotations;

namespace LearnHub.Models;

/// <summary>A rating and comment left by a member on a learning resource.</summary>
public class Review
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public int ResourceId { get; set; }
    public LearningResource? Resource { get; set; }

    [Range(1, 5)]
    [Display(Name = "Rating")]
    public int Rating { get; set; } = 5;

    [Required, StringLength(800)]
    [Display(Name = "Your review")]
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
