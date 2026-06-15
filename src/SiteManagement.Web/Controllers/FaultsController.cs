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
public class FaultsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public FaultsController(AppDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // Herkes arızaları görsün
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var buildingId = await BuildingContext.GetCurrentBuildingIdAsync(_db, _userManager, User);
        if (buildingId == null) return RedirectToAction("Index", "Home");

        var faults = await _db.FaultReports
            .Where(x => x.BuildingId == buildingId.Value)
            .OrderBy(x => x.IsResolved)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(faults);
    }

    // Herkes arıza ekleyebilir
    [HttpGet]
    public IActionResult Create()
    {
        return View(new FaultCreateVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FaultCreateVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var buildingId = await BuildingContext.GetCurrentBuildingIdAsync(_db, _userManager, User);
        if (buildingId == null) return RedirectToAction("Index", "Home");

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var fault = new FaultReport(buildingId.Value, vm.Title, vm.Description, user.Id);

        _db.FaultReports.Add(fault);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Sadece yönetici çözülmüş işaretlesin
    [Authorize(Roles = "Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkResolved(int id)
    {
        var buildingId = await BuildingContext.GetCurrentBuildingIdAsync(_db, _userManager, User);
        if (buildingId == null) return RedirectToAction("Index", "Home");

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var fault = await _db.FaultReports
            .FirstOrDefaultAsync(x => x.Id == id && x.BuildingId == buildingId.Value);

        if (fault == null) return NotFound();

        fault.MarkResolved(user.Id);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}