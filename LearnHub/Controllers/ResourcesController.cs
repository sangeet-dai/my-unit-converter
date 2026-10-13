using System.Security.Claims;
using LearnHub.Data;
using LearnHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Controllers;

/// <summary>Browse/search the catalogue and view a single resource with its reviews.</summary>
public class ResourcesController : UserAwareController
{
    public ResourcesController(AppDbContext db) : base(db) { }

    // GET: /Resources
    public async Task<IActionResult> Index(CatalogFilterViewModel filter)
    {
        var query = Db.Resources
            .Include(r => r.Category)
            .Include(r => r.Reviews)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            var q = filter.Query.Trim().ToLower();
            query = query.Where(r =>
                r.Title.ToLower().Contains(q) ||
                r.Summary.ToLower().Contains(q) ||
                r.Description.ToLower().Contains(q));
        }
        if (filter.CategoryId.HasValue)
            query = query.Where(r => r.CategoryId == filter.CategoryId.Value);
        if (!string.IsNullOrWhiteSpace(filter.ResourceType))
            query = query.Where(r => r.ResourceType == filter.ResourceType);
        if (!string.IsNullOrWhiteSpace(filter.Level))
            query = query.Where(r => r.Level == filter.Level);

        query = filter.Sort switch
        {
            "rating" => query.OrderByDescending(r => r.Reviews.Average(x => (double?)x.Rating) ?? 0),
            "duration" => query.OrderBy(r => r.DurationMinutes),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };

        filter.TotalCount = await query.CountAsync();
        filter.TotalPages = Math.Max(1, (int)Math.Ceiling(filter.TotalCount / (double)CatalogFilterViewModel.PageSize));
        filter.Page = Math.Clamp(filter.Page, 1, filter.TotalPages);
        filter.Items = await query
            .Skip((filter.Page - 1) * CatalogFilterViewModel.PageSize)
            .Take(CatalogFilterViewModel.PageSize)
            .ToListAsync();
        filter.Categories = await Db.Categories.OrderBy(c => c.Name).ToListAsync();

        return View(filter);
    }

    // GET: /Resources/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var resource = await Db.Resources
            .Include(r => r.Category)
            .Include(r => r.Reviews).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (resource is null) return NotFound();

        var me = await CurrentUserAsync();
        ViewBag.MyEnrollment = me is null
            ? null
            : await Db.Enrollments.FirstOrDefaultAsync(e => e.UserId == me.Id && e.ResourceId == id);
        ViewBag.HasReviewed = me is null
            ? false
            : await Db.Reviews.AnyAsync(r => r.UserId == me.Id && r.ResourceId == id);

        ViewBag.Related = await Db.Resources
            .Include(r => r.Category)
            .Include(r => r.Reviews)
            .Where(r => r.CategoryId == resource.CategoryId && r.Id != resource.Id)
            .Take(3)
            .ToListAsync();

        return View(resource);
    }

    // POST: /Resources/SubmitReview
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitReview(int resourceId, ReviewInputViewModel model)
    {
        var resource = await Db.Resources.FindAsync(resourceId);
        if (resource is null) return NotFound();

        var me = await CurrentUserAsync();
        if (me is null)
            return RedirectToAction(nameof(AccountController.Login),
                new { returnUrl = Url.Action(nameof(Details), "Resources", new { id = resourceId }) });

        if (string.IsNullOrWhiteSpace(model.Comment) || model.Comment.Trim().Length < 5 ||
            model.Rating < 1 || model.Rating > 5)
        {
            TempData["Error"] = "Please choose a rating between 1 and 5 and write at least 5 characters.";
            return RedirectToAction(nameof(Details), new { id = resourceId });
        }

        if (await Db.Reviews.AnyAsync(r => r.UserId == me.Id && r.ResourceId == resourceId))
        {
            TempData["Error"] = "You have already reviewed this resource.";
            return RedirectToAction(nameof(Details), new { id = resourceId });
        }

        Db.Reviews.Add(new Review
        {
            UserId = me.Id,
            ResourceId = resourceId,
            Rating = model.Rating,
            Comment = model.Comment.Trim()
        });
        await Db.SaveChangesAsync();

        TempData["Success"] = "Thank you! Your review has been published.";
        return RedirectToAction(nameof(Details), new { id = resourceId });
    }

    // POST: /Resources/DeleteReview
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await Db.Reviews.FindAsync(id);
        if (review is null) return NotFound();

        var me = await CurrentUserAsync();
        if (me is null || (review.UserId != me.Id && !IsAdmin))
            return RedirectToAction(nameof(AccountController.AccessDenied));

        Db.Reviews.Remove(review);
        await Db.SaveChangesAsync();
        TempData["Success"] = "Review deleted.";
        return RedirectToAction(nameof(Details), new { id = review.ResourceId });
    }
}
