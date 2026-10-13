using LearnHub.Data;
using LearnHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Controllers;

public class HomeController : UserAwareController
{
    public HomeController(AppDbContext db) : base(db) { }

    // GET: /Home/Index
    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel
        {
            FeaturedResources = await Db.Resources
                .Include(r => r.Category)
                .Include(r => r.Reviews)
                .Where(r => r.IsFeatured)
                .OrderByDescending(r => r.Id)
                .Take(3)
                .ToListAsync(),
            LatestResources = await Db.Resources
                .Include(r => r.Category)
                .Include(r => r.Reviews)
                .OrderByDescending(r => r.CreatedAt)
                .Take(6)
                .ToListAsync(),
            Categories = await Db.Categories
                .OrderBy(c => c.Name)
                .ToListAsync(),
            TotalResources = await Db.Resources.CountAsync(),
            TotalMembers = await Db.Users.CountAsync(u => u.Role == Roles.Member),
            TotalReviews = await Db.Reviews.CountAsync(),
            RecentReviews = await Db.Reviews
                .Include(r => r.User)
                .Include(r => r.Resource)
                .OrderByDescending(r => r.CreatedAt)
                .Take(3)
                .ToListAsync()
        };
        return View(model);
    }

    // GET: /Home/StatusCode?code=404
    public IActionResult StatusCode(int code)
    {
        Response.StatusCode = code;
        ViewData["Title"] = code == 404 ? "Page not found" : "Something went wrong";
        ViewBag.Code = code;
        return View();
    }

    // GET: /Home/About
    public IActionResult About() => View();

    // GET: /Home/Contact
    [HttpGet]
    public IActionResult Contact() => View(new ContactFormViewModel());

    // POST: /Home/Contact
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        Db.Messages.Add(new ContactMessage
        {
            Name = model.Name.Trim(),
            Email = model.Email.Trim(),
            Subject = model.Subject.Trim(),
            Message = model.Message.Trim()
        });
        await Db.SaveChangesAsync();

        TempData["Success"] = "Thank you for your message — our team will reply shortly.";
        return RedirectToAction(nameof(Contact));
    }
}

public class HomeViewModel
{
    public List<LearningResource> FeaturedResources { get; set; } = new();
    public List<LearningResource> LatestResources { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Review> RecentReviews { get; set; } = new();
    public int TotalResources { get; set; }
    public int TotalMembers { get; set; }
    public int TotalReviews { get; set; }
}
