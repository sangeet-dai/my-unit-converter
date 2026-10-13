using System.Security.Claims;
using LearnHub.Data;
using LearnHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Controllers;

/// <summary>Shared helpers: loads the signed-in user and offers a typed Db.</summary>
public abstract class BaseController : Controller
{
    protected readonly AppDbContext Db;

    protected BaseController(AppDbContext db) => Db = db;

    /// <summary>The ApplicationUser record for the signed-in identity, or null.</summary>
    protected async Task<ApplicationUser?> CurrentUserAsync()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idValue, out int id)) return null;
        return await Db.Users.FindAsync(id);
    }

    protected bool IsAdmin => User.IsInRole(Roles.Admin);
}

/// <summary>Ensures a fresh ApplicationUser row is available to every action result.</summary>
public abstract class UserAwareController : BaseController
{
    protected UserAwareController(AppDbContext db) : base(db) { }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ViewBag.CurrentUser = await CurrentUserAsync();
        await next();
    }
}
