using LearnHub.Data;
using LearnHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Controllers;

/// <summary>
/// Administrator module: requires a valid admin login.
/// Manages website content (learning resources) and the database records
/// (members, reviews, contact messages).
/// </summary>
[Authorize(Roles = Roles.Admin)]
public class AdminController : UserAwareController
{
    public AdminController(AppDbContext db) : base(db) { }

    // GET: /Admin/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var model = new AdminDashboardViewModel
        {
            TotalResources = await Db.Resources.CountAsync(),
            TotalMembers = await Db.Users.CountAsync(u => u.Role == Roles.Member),
            TotalEnrollments = await Db.Enrollments.CountAsync(),
            UnreadMessages = await Db.Messages.CountAsync(m => !m.IsRead),
            RecentMessages = await Db.Messages.OrderByDescending(m => m.CreatedAt).Take(5).ToListAsync(),
            TopResources = await Db.Resources
                .Include(r => r.Enrollments)
                .Include(r => r.Reviews)
                .OrderByDescending(r => r.Enrollments.Count)
                .Take(5)
                .ToListAsync()
        };
        return View(model);
    }

    // ===================== LEARNING RESOURCES (CRUD) =====================

    // GET: /Admin/Resources
    public async Task<IActionResult> Resources(string? q)
    {
        var query = Db.Resources.Include(r => r.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(r => r.Title.ToLower().Contains(q.ToLower()));
        ViewBag.Query = q;
        return View(await query.OrderByDescending(r => r.Id).ToListAsync());
    }

    // GET: /Admin/CreateResource
    [HttpGet]
    public async Task<IActionResult> CreateResource() =>
        View("ResourceForm", await BuildFormAsync(new ResourceFormViewModel()));

    // POST: /Admin/CreateResource
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateResource(ResourceFormViewModel model, IFormFile? PictureUrl)
    {
        if (!ModelState.IsValid)
            return View("ResourceForm", await BuildFormAsync(model));

        var me = await CurrentUserAsync();
        var resource = new LearningResource
        {
            Title = model.Title.Trim(),
            Summary = model.Summary.Trim(),
            Description = model.Description.Trim(),
            Objectives = string.IsNullOrWhiteSpace(model.Objectives) ? null : model.Objectives.Trim(),
            CategoryId = model.CategoryId,
            ResourceType = model.ResourceType,
            Level = model.Level,
            DurationMinutes = model.DurationMinutes,
            ThumbnailUrl = string.IsNullOrWhiteSpace(model.ThumbnailUrl) ? null : model.ThumbnailUrl.Trim(),
            ResourceUrl = string.IsNullOrWhiteSpace(model.ResourceUrl) ? null : model.ResourceUrl.Trim(),
            CategoryLogoUrl = model.CategoryLogoUrl?.Trim(),
            IsFeatured = model.IsFeatured,
            PublisherId = me?.Id
        };

        // ---- resource picture upload ----
        if (PictureUrl != null && PictureUrl.Length > 0)
        {
            if (PictureUrl.Length > 3_000_000)
                ModelState.AddModelError(nameof(PictureUrl), "Picture must be under 3 MB.");
            else if (!IsImage(PictureUrl))
                ModelState.AddModelError(nameof(PictureUrl), "Only JPG, PNG or GIF pictures are allowed.");
            else
            {
                var ext = Path.GetExtension(PictureUrl.FileName).ToLowerInvariant();
                resource.PictureUrl = "/images/resource/" + resource.Id + ext;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", resource.PictureUrl.TrimStart('/'));
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var stream = new FileStream(path, FileMode.Create);
                await PictureUrl.CopyToAsync(stream);
            }
        }
        Db.Resources.Add(resource);
        await Db.SaveChangesAsync();
        TempData["Success"] = "Resource created successfully.";
        return RedirectToAction(nameof(Resources));
    }

    // GET: /Admin/EditResource/5
    [HttpGet]
    public async Task<IActionResult> EditResource(int id)
    {
        var resource = await Db.Resources.FindAsync(id);
        if (resource is null) return NotFound();

        var model = new ResourceFormViewModel
        {
            Id = resource.Id,
            Title = resource.Title,
            Summary = resource.Summary,
            Description = resource.Description,
            Objectives = resource.Objectives,
            CategoryId = resource.CategoryId,
            ResourceType = resource.ResourceType,
            Level = resource.Level,
            DurationMinutes = resource.DurationMinutes,
            ThumbnailUrl = resource.ThumbnailUrl,
            ResourceUrl = resource.ResourceUrl,
            PictureUrl = resource.PictureUrl,
            CategoryLogoUrl = resource.CategoryLogoUrl,
            IsFeatured = resource.IsFeatured
        };
        return View("ResourceForm", await BuildFormAsync(model));
    }

    // POST: /Admin/EditResource/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditResource(int id, ResourceFormViewModel model, IFormFile? PictureUrl)
    {
        var resource = await Db.Resources.FindAsync(id);
        if (resource is null) return NotFound();
        model.Id = resource.Id;

        if (!ModelState.IsValid)
            return View("ResourceForm", await BuildFormAsync(model));

        resource.Title = model.Title.Trim();
        resource.Summary = model.Summary.Trim();
        resource.Description = model.Description.Trim();
        resource.Objectives = string.IsNullOrWhiteSpace(model.Objectives) ? null : model.Objectives.Trim();
        resource.CategoryId = model.CategoryId;
        resource.ResourceType = model.ResourceType;
        resource.Level = model.Level;
        resource.DurationMinutes = model.DurationMinutes;
        resource.ThumbnailUrl = string.IsNullOrWhiteSpace(model.ThumbnailUrl) ? null : model.ThumbnailUrl.Trim();
        resource.ResourceUrl = string.IsNullOrWhiteSpace(model.ResourceUrl) ? null : model.ResourceUrl.Trim();
        resource.PictureUrl = model.PictureUrl?.Trim();
        resource.CategoryLogoUrl = model.CategoryLogoUrl?.Trim();
        resource.IsFeatured = model.IsFeatured;
        resource.UpdatedAt = DateTime.UtcNow;

        // ---- resource picture upload ----
        if (PictureUrl != null && PictureUrl.Length > 0)
        {
            if (PictureUrl.Length > 3_000_000)
                ModelState.AddModelError(nameof(PictureUrl), "Picture must be under 3 MB.");
            else if (!IsImage(PictureUrl))
                ModelState.AddModelError(nameof(PictureUrl), "Only JPG, PNG or GIF pictures are allowed.");
            else
            {
                var ext = Path.GetExtension(PictureUrl.FileName).ToLowerInvariant();
                resource.PictureUrl = "/images/resource/" + resource.Id + ext;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", resource.PictureUrl.TrimStart('/'));
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var stream = new FileStream(path, FileMode.Create);
                await PictureUrl.CopyToAsync(stream);
            }
        }
        await Db.SaveChangesAsync();

        TempData["Success"] = "Resource updated successfully.";
        return RedirectToAction(nameof(Resources));
    }

    // POST: /Admin/DeleteResource/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteResource(int id)
    {
        var resource = await Db.Resources
            .Include(r => r.Enrollments)
            .Include(r => r.Reviews)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (resource is null) return NotFound();

        Db.Resources.Remove(resource); // enrolments & reviews cascade
        await Db.SaveChangesAsync();
        TempData["Success"] = resource.Title + " has been deleted.";
        return RedirectToAction(nameof(Resources));
    }

    private async Task<ResourceFormViewModel> BuildFormAsync(ResourceFormViewModel model)
    {
        model.Categories = await Db.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.CategoryList = new SelectList(model.Categories, "Id", "Name", model.CategoryId);
        return model;
    }

    // ===================== MEMBERS =====================

    // GET: /Admin/Members
    public async Task<IActionResult> Members() =>
        View(await Db.Users
            .Include(u => u.Enrollments)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync());

    // POST: /Admin/ToggleRole/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleRole(int id)
    {
        var user = await Db.Users.FindAsync(id);
        if (user is null) return NotFound();
        if (user.Email == "admin@learnhub.test")
        {
            TempData["Error"] = "The primary administrator's role cannot be changed.";
            return RedirectToAction(nameof(Members));
        }
        user.Role = user.Role == Roles.Admin ? Roles.Member : Roles.Admin;
        await Db.SaveChangesAsync();
        TempData["Success"] = user.FullName + " is now a " + user.Role + ".";
        return RedirectToAction(nameof(Members));
    }

    // POST: /Admin/DeleteMember/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMember(int id)
    {
        var user = await Db.Users
            .Include(u => u.Enrollments)
            .Include(u => u.Reviews)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();

        if (user.Email == "admin@learnhub.test")
        {
            TempData["Error"] = "The primary administrator cannot be deleted.";
            return RedirectToAction(nameof(Members));
        }

        Db.Users.Remove(user);
        await Db.SaveChangesAsync();
        TempData["Success"] = user.FullName + " and all related records were deleted.";
        return RedirectToAction(nameof(Members));
    }

    // ===================== REVIEWS =====================

    // GET: /Admin/Reviews
    public async Task<IActionResult> Reviews() =>
        View(await Db.Reviews
            .Include(r => r.User)
            .Include(r => r.Resource)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync());

    // POST: /Admin/DeleteReview/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await Db.Reviews.FindAsync(id);
        if (review is null) return NotFound();
        Db.Reviews.Remove(review);
        await Db.SaveChangesAsync();
        TempData["Success"] = "Review removed.";
        return RedirectToAction(nameof(Reviews));
    }

    // ===================== CONTACT MESSAGES =====================

    // GET: /Admin/Messages
    public async Task<IActionResult> Messages() =>
        View(await Db.Messages.OrderByDescending(m => m.CreatedAt).ToListAsync());

    // POST: /Admin/ToggleMessage/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleMessage(int id)
    {
        var message = await Db.Messages.FindAsync(id);
        if (message is null) return NotFound();
        message.IsRead = !message.IsRead;
        await Db.SaveChangesAsync();
        return RedirectToAction(nameof(Messages));
    }

    // POST: /Admin/DeleteMessage/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var message = await Db.Messages.FindAsync(id);
        if (message is null) return NotFound();
        Db.Messages.Remove(message);
        await Db.SaveChangesAsync();
        TempData["Success"] = "Message deleted.";
        return RedirectToAction(nameof(Messages));
    }

    // ---- helper: only accept real image files ----
    private static bool IsImage(IFormFile file)
    {
        var ok = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        return ok.Contains(Path.GetExtension(file.FileName).ToLowerInvariant());
    }
}

public class AdminDashboardViewModel
{
    public int TotalResources { get; set; }
    public int TotalMembers { get; set; }
    public int TotalEnrollments { get; set; }
    public int UnreadMessages { get; set; }
    public List<ContactMessage> RecentMessages { get; set; } = new();
    public List<LearningResource> TopResources { get; set; } = new();
}
