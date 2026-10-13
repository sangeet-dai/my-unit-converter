using System.ComponentModel.DataAnnotations;

namespace LearnHub.Models;

/// <summary>A registered user of the learning platform (member or administrator).</summary>
public class ApplicationUser
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>"Member" or "Admin".</summary>
    [Required, StringLength(20)]
    public string Role { get; set; } = Roles.Member;

    [StringLength(120)]
    [Display(Name = "Institution / Company")]
    public string? Organisation { get; set; }

    [StringLength(500)]
    [Display(Name = "About you")]
    public string? Bio { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>Path to the member's profile picture, stored in wwwroot/images/avatars/.</summary>
    [StringLength(300)]
    [Display(Name = "Profile picture")]
    public string? AvatarUrl { get; set; }
}

public static class Roles
{
    public const string Member = "Member";
    public const string Admin = "Admin";
}
