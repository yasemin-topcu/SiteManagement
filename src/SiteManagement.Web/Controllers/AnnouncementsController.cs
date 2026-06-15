using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteManagement.Domain.Entities;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Infrastructure.Persistence;
using SiteManagement.Web.Helpers;
using SiteManagement.Web.ViewModels;

namespace SiteManagement.Web.Controllers;

[Authorize]
public class AnnouncementsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public AnnouncementsController(AppDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // Hem Manager hem Resident: duyuruları görür
    public async Task<IActionResult> Index()
    {
        var buildingId = await BuildingContext.GetCurrentBuildingIdAsync(_db, _userManager, User);
        if (buildingId == null) return RedirectToAction("Index", "Home");

        var list = await _db.Announcements
            .Where(x => x.BuildingId == buildingId.Value)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(list);
    }

    [Authorize(Roles = "Manager")]
    [HttpGet]
    public IActionResult Create() => View(new AnnouncementCreateVm());

    [Authorize(Roles = "Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AnnouncementCreateVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var buildingId = await BuildingContext.GetCurrentBuildingIdAsync(_db, _userManager, User);
        if (buildingId == null) return RedirectToAction("Index", "Home");

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var ann = new Announcement(buildingId.Value, vm.Title, vm.Content, user.Id);
        _db.Announcements.Add(ann);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}