using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Abstractions;
using SiteManagement.Domain.Entities;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Infrastructure.Persistence;
using SiteManagement.Web.ViewModels;

namespace SiteManagement.Web.Controllers;

[Authorize(Roles = "Manager")]
public class BuildingController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly IJoinCodeGenerator _joinCodeGenerator;

    public BuildingController(AppDbContext db, UserManager<AppUser> userManager, IJoinCodeGenerator joinCodeGenerator)
    {
        _db = db;
        _userManager = userManager;
        _joinCodeGenerator = joinCodeGenerator;
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateBuildingVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBuildingVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        // JoinCode benzersiz olmalı -> çakışma olursa yeniden üret
        string code;
        do
        {
            code = _joinCodeGenerator.Generate(6);
        } while (await _db.Buildings.AnyAsync(x => x.JoinCode == code));

        var building = new Building(vm.Name, code);
        await _db.Buildings.AddAsync(building);
        await _db.SaveChangesAsync();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        // Yönetici ile apartmanı ilişkilendir
        var link = new BuildingUser(building.Id, user.Id, "Manager");
        await _db.BuildingUsers.AddAsync(link);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = building.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var building = await _db.Buildings.FirstOrDefaultAsync(x => x.Id == id);
        if (building == null) return NotFound();

        return View(building);
    }
}