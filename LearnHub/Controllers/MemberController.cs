using LearnHub.Data;
using LearnHub.Helpers;
using LearnHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Controllers;

/// <summary>
/// Registered-member module: requires a valid login.
/// Manages the member's own activities - enrolments, progress, reviews and profile.
/// </summary>
[Authorize]
public class MemberController : UserAwareController
{
    public MemberController(AppDbContext db) : base(db) { }

    // GET: /Member/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var me = await CurrentUserAsync();
        if (me is null) return RedirectToAction(nameof(AccountController.Login));

        var enrollments = await Db.Enrollments
            .Include(e => e.Resource).ThenInclude(r => r.Category)
            .Where(e => e.UserId == me.Id)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

        var model = new MemberDashboardViewModel
        {
            User = me,
            Enrollments = enrollments,
            MyReviews = await Db.Reviews
                .Include(r => r.Resource)
                .Where(r => r.UserId == me.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(),
            CompletedCount = enrollments.Count(e => e.Status == EnrollmentStatus.Completed),
            InProgressCount = enrollments.Count(e => e.Status == EnrollmentStatus.InProgress)
        };
        return View(model);
    }

    // POST: /Member/Enroll/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int resourceId)
    {
        var me = await CurrentUserAsync();
        if (me is null) return RedirectToAction(nameof(AccountController.Login));

        var resource = await Db.Resources.FindAsync(resourceId);
        if (resource is null) return NotFound();

        if (!await Db.Enrollments.AnyAsync(e => e.UserId == me.Id && e.ResourceId == resourceId))
        {
            Db.Enrollments.Add(new Enrollment { UserId = me.Id, ResourceId = resourceId });
            await Db.SaveChangesAsync();
            TempData["Success"] = "You are now enrolled in " + resource.Title + ".";
        }
        else
        {
            TempData["Error"] = "You are already enrolled in this resource.";
        }
        return RedirectToAction(nameof(Dashboard));
    }

    // POST: /Member/UpdateProgress/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProgress(int id, int percent)
    {
        var me = await CurrentUserAsync();
        if (me is null) return RedirectToAction(nameof(AccountController.Login));

        var enrollment = await Db.Enrollments
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == me.Id);
        if (enrollment is null) return NotFound();

        enrollment.ProgressPercent = Math.Clamp(percent, 0, 100);
        if (enrollment.ProgressPercent == 100)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletedAt ??= DateTime.UtcNow;
        }
        else
        {
            enrollment.Status = EnrollmentStatus.InProgress;
            enrollment.CompletedAt = null;
        }
        await Db.SaveChangesAsync();
        TempData["Success"] = "Progress updated.";
        return RedirectToAction(nameof(Dashboard));
    }

    // POST: /Member/Unenroll/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Unenroll(int id)
    {
        var me = await CurrentUserAsync();
        if (me is null) return RedirectToAction(nameof(AccountController.Login));

        var enrollment = await Db.Enrollments
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == me.Id);
        if (enrollment is null) return NotFound();

        Db.Enrollments.Remove(enrollment);
        await Db.SaveChangesAsync();
        TempData["Success"] = "Enrolment removed.";
        return RedirectToAction(nameof(Dashboard));
    }

    // GET: /Member/Profile
    public async Task<IActionResult> Profile()
    {
        var me = await CurrentUserAsync();
        if (me is null) return RedirectToAction(nameof(AccountController.Login));

        return View(new ProfileInputViewModel
        {
            Id = me.Id,
            FullName = me.FullName,
            Organisation = me.Organisation,
            Bio = me.Bio,
            Picture = null,
            AvatarUrl = me.AvatarUrl
        });
    }

    // POST: /Member/Profile
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileInputViewModel model)
    {
        var me = await CurrentUserAsync();
        if (me is null) return RedirectToAction(nameof(AccountController.Login));
        if (!ModelState.IsValid) return View(model);

        me.FullName = model.FullName.Trim();
        me.Organisation = model.Organisation?.Trim();
        me.Bio = model.Bio?.Trim();

        // ---- profile picture upload ----
        if (model.Picture is not null && model.Picture.Length > 0)
        {
            if (model.Picture.Length > 3_000_000)
                ModelState.AddModelError(nameof(model.Picture), "Picture must be under 3 MB.");
            else if (!IsImage(model.Picture))
                ModelState.AddModelError(nameof(model.Picture), "Only JPG, PNG or GIF pictures are allowed.");
            else
            {
                var ext = Path.GetExtension(model.Picture.FileName).ToLowerInvariant();
                me.AvatarUrl = "/images/avatars/" + me.Id + ext;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", me.AvatarUrl.TrimStart('/'));
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var stream = new FileStream(path, FileMode.Create);
                await model.Picture.CopyToAsync(stream);
                TempData["Success"] = "Profile picture updated.";
            }
        }

        await Db.SaveChangesAsync();
        return RedirectToAction(nameof(Profile));
    }

    // GET: /Member/ChangePassword
    [HttpGet]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    // POST: /Member/ChangePassword
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var me = await CurrentUserAsync();
        if (me is null) return RedirectToAction(nameof(AccountController.Login));
        if (!ModelState.IsValid) return View(model);

        if (!PasswordHasher.Verify(model.CurrentPassword, me.PasswordHash))
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Your current password is incorrect.");
            return View(model);
        }

        me.PasswordHash = PasswordHasher.Hash(model.NewPassword);
        await Db.SaveChangesAsync();
        TempData["Success"] = "Password changed successfully.";
        return RedirectToAction(nameof(Dashboard));
    }

    // ---- helper: only accept real image files ----
    private static bool IsImage(IFormFile file)
    {
        var ok = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        return ok.Contains(Path.GetExtension(file.FileName).ToLowerInvariant());
    }
}

public class MemberDashboardViewModel
{
    public ApplicationUser User { get; set; } = new();
    public List<Enrollment> Enrollments { get; set; } = new();
    public List<Review> MyReviews { get; set; } = new();
    public int CompletedCount { get; set; }
    public int InProgressCount { get; set; }
}
