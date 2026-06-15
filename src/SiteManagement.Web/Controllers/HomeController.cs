using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Infrastructure.Persistence;
using SiteManagement.Web.Models;

namespace SiteManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public HomeController(
        ILogger<HomeController> logger,
        AppDbContext db,
        UserManager<AppUser> userManager)
    {
        _logger = logger;
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // Giriş yapılmadıysa normal landing page
        if (!(User.Identity?.IsAuthenticated ?? false))
            return View();

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return View();

        // Yönetici ise: apartmanı var mı?
        if (User.IsInRole("Manager"))
        {
            var hasBuilding = await _db.BuildingUsers
                .AnyAsync(x => x.UserId == user.Id && x.MemberType == "Manager");

            if (!hasBuilding)
                return RedirectToAction("Create", "Building");

            return RedirectToAction("Index", "Dashboard");
        }

        // Daire sakini ise: apartmana bağlı mı?
        var hasLink = await _db.BuildingUsers
            .AnyAsync(x => x.UserId == user.Id && x.MemberType == "Resident");

        if (!hasLink)
            return RedirectToAction("Register", "Account"); // istersen Join sayfasına yönlendireceğiz

        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}