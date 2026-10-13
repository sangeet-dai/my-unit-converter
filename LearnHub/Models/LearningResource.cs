using System.ComponentModel.DataAnnotations;

namespace LearnHub.Models;

/// <summary>A digital learning resource (video, article, quiz, virtual lab, ...).</summary>
public class LearningResource
{
    public int Id { get; set; }

    [Required, StringLength(140)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(260)]
    [Display(Name = "Short summary")]
    public string Summary { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Full description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Learning objectives")]
    public string? Objectives { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    /// <summary>Video, Article, Quiz, Virtual Lab, Simulation, Interactive.</summary>
    [Required, StringLength(40)]
    public string ResourceType { get; set; } = ResourceTypeNames.Article;

    /// <summary>Beginner, Intermediate, Advanced.</summary>
    [Required, StringLength(30)]
    public string Level { get; set; } = LevelNames.Beginner;

    [Range(5, 10000)]
    [Display(Name = "Estimated duration (minutes)")]
    public int DurationMinutes { get; set; } = 30;

    [StringLength(300)]
    [Display(Name = "Thumbnail image URL")]
    public string? ThumbnailUrl { get; set; }

    [StringLength(600)]
    [Display(Name = "Resource link")]
    public string? ResourceUrl { get; set; }

    /// <summary>File name (without extension) of the accompanying media clip, when the resource has one.</summary>
    [StringLength(80)]
    [Display(Name = "Media clip")]
    public string? MediaSlug { get; set; }

    public bool IsFeatured { get; set; }

    /// <summary>Image override used by the card when the resource has a real picture.</summary>
    [StringLength(300)]
    [Display(Name = "Resource picture")]
    public string? PictureUrl { get; set; }

    /// <summary>Category logo used to illustrate this resource.</summary>
    [StringLength(300)]
    [Display(Name = "Category logo")]
    public string? CategoryLogoUrl { get; set; }

    /// <summary>Administrator who published this resource.</summary>
    public int? PublisherId { get; set; }
    public ApplicationUser? Publisher { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>Average member rating (computed when loaded with reviews).</summary>
    public double AverageRating =>
        Reviews.Count > 0 ? Math.Round(Reviews.Average(r => r.Rating), 1) : 0;
}

public static class ResourceTypeNames
{
    public const string Video = "Video";
    public const string Article = "Article";
    public const string Quiz = "Self-Assessment Quiz";
    public const string VirtualLab = "Virtual Lab";
    public const string Simulation = "Simulation";
    public const string Interactive = "Interactive Object";
}

public static class LevelNames
{
    public const string Beginner = "Beginner";
    public const string Intermediate = "Intermediate";
    public const string Advanced = "Advanced";
}
