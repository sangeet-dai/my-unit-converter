using System.Security.Claims;
using LearnHub.Data;
using LearnHub.Helpers;
using LearnHub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Controllers;

/// <summary>Public registration, login and logout.</summary>
public class AccountController : UserAwareController
{
    public AccountController(AppDbContext db) : base(db) { }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register() => View();

    // POST: /Account/Register
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var email = model.Email.Trim().ToLower();
        if (await Db.Users.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            FullName = model.FullName.Trim(),
            Email = email,
            Organisation = model.Organisation?.Trim(),
            PasswordHash = PasswordHasher.Hash(model.Password),
            Role = Roles.Member
        };
        Db.Users.Add(user);
        await Db.SaveChangesAsync();

        await SignInAsync(user, isPersistent: false);
        TempData["Success"] = $"Welcome to LearnHub, {user.FullName}! Your account has been created.";
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null) =>
        View(new LoginViewModel { ReturnUrl = returnUrl });

    // POST: /Account/Login
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var email = model.Email.Trim().ToLower();
        var user = await Db.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user is null || !user.IsActive || !PasswordHasher.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        await SignInAsync(user, model.RememberMe);

        // Send the user back where they were headed, otherwise to the right dashboard
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return user.Role == Roles.Admin
            ? RedirectToAction("Dashboard", "Admin")
            : RedirectToAction("Dashboard", "Member");
    }

    // POST: /Account/Logout
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Success"] = "You have been signed out.";
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    // GET: /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied() => View();

    private async Task SignInAsync(ApplicationUser user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = isPersistent });
    }
}
