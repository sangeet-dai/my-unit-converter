using System.ComponentModel.DataAnnotations;
using LearnHub.Models;

namespace LearnHub.Models;

// ---------- Account ----------
public class RegisterViewModel
{
    [Required, StringLength(80, MinimumLength = 2)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [StringLength(120)]
    [Display(Name = "Institution / Company")]
    public string? Organisation { get; set; }

    [Required, StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter and one digit.")]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms to register.")]
    public bool AcceptTerms { get; set; }
}

public class LoginViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class ProfileInputViewModel
{
    public int Id { get; set; }

    /// <summary>Current profile picture path (set when the image was already uploaded).</summary>
    [StringLength(300)]
    [Display(Name = "Profile picture")]
    public string? AvatarUrl { get; set; }

    [Required, StringLength(80, MinimumLength = 2)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(120)]
    [Display(Name = "Institution / Company")]
    public string? Organisation { get; set; }

    [StringLength(500)]
    [Display(Name = "About you")]
    public string? Bio { get; set; }

    /// <summary>Uploaded profile picture (saved to wwwroot/images/avatars/).</summary>
    [Display(Name = "Profile picture")]
    public IFormFile? Picture { get; set; }
}

public class ChangePasswordViewModel
{
    [Required, DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter and one digit.")]
    [Display(Name = "New password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation do not match.")]
    [Display(Name = "Confirm new password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

// ---------- Resources ----------
public class ResourceFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(140, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(260, MinimumLength = 10)]
    [Display(Name = "Short summary")]
    public string Summary { get; set; } = string.Empty;

    [Required, StringLength(8000, MinimumLength = 30)]
    [Display(Name = "Full description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Learning objectives")]
    public string? Objectives { get; set; }

    [Required, Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required, Display(Name = "Resource type")]
    public string ResourceType { get; set; } = ResourceTypeNames.Article;

    [Required, Display(Name = "Level")]
    public string Level { get; set; } = LevelNames.Beginner;

    [Range(5, 10000)]
    [Display(Name = "Estimated duration (minutes)")]
    public int DurationMinutes { get; set; } = 30;

    [StringLength(300)]
    [Display(Name = "Thumbnail image URL")]
    public string? ThumbnailUrl { get; set; }

    [StringLength(600), Url]
    [Display(Name = "Resource link")]
    public string? ResourceUrl { get; set; }

    [Display(Name = "Feature on the home page")]
    public bool IsFeatured { get; set; }

    /// <summary>Resource picture override (wwwroot/images/resource/).</summary>
    [StringLength(300)]
    [Display(Name = "Resource picture")]
    public string? PictureUrl { get; set; }

    /// <summary>Category logo for display in forms and admin tables.</summary>
    [StringLength(300)]
    [Display(Name = "Category logo")]
    public string? CategoryLogoUrl { get; set; }

    public IEnumerable<Category>? Categories { get; set; }
}

public class CatalogFilterViewModel
{
    public string? Query { get; set; }
    public int? CategoryId { get; set; }
    public string? ResourceType { get; set; }
    public string? Level { get; set; }
    public string? Sort { get; set; } // newest | rating | duration
    public int Page { get; set; } = 1;
    public const int PageSize = 6;

    public int TotalPages { get; set; }
    public List<LearningResource> Items { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public int TotalCount { get; set; }
}

// ---------- Member ----------
public class ReviewInputViewModel
{
    [Required, Range(1, 5)]
    public int Rating { get; set; } = 5;

    [Required, StringLength(800, MinimumLength = 5)]
    [Display(Name = "Your review")]
    public string Comment { get; set; } = string.Empty;
}

// ---------- Contact ----------
public class ContactFormViewModel
{
    [Required, StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(140, MinimumLength = 3)]
    public string Subject { get; set; } = string.Empty;

    [Required, StringLength(2000, MinimumLength = 10)]
    public string Message { get; set; } = string.Empty;
}
