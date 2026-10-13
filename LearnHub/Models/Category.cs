using System.ComponentModel.DataAnnotations;

namespace LearnHub.Models;

/// <summary>Subject area used to organise learning resources.</summary>
public class Category
{
    public int Id { get; set; }

    [Required, StringLength(60)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }

    /// <summary>Font-awesome style icon class used by the theme.</summary>
    [StringLength(60)]
    public string IconClass { get; set; } = "bi bi-book";

    public ICollection<LearningResource> Resources { get; set; } = new List<LearningResource>();

    /// <summary>Path to the category logo (wwwroot/images/category/).</summary>
    [StringLength(300)]
    [Display(Name = "Logo")]
    public string? LogoUrl { get; set; }
}
