using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Infrastructure.Persistence;
using SiteManagement.Web.ViewModels;

namespace SiteManagement.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public DashboardController(AppDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        // Şimdilik: kullanıcının ilk bağlı olduğu apartman
        var buildingId = await _db.BuildingUsers
            .Where(x => x.UserId == user.Id)
            .Select(x => (int?)x.BuildingId)
            .FirstOrDefaultAsync();

        if (buildingId == null)
            return RedirectToAction("Index", "Home"); // veya Join/Register

        var building = await _db.Buildings.FirstOrDefaultAsync(x => x.Id == buildingId.Value);
        if (building == null)
            return RedirectToAction("Index", "Home");

        // Toplam sayılar (küçük istatistik gibi)
        var totalAnnouncements = await _db.Announcements.CountAsync(a => a.BuildingId == building.Id);
        var totalFees = await _db.Fees.CountAsync(f => f.BuildingId == building.Id);

        // Son 3 duyuru
        var latestAnnouncements = await _db.Announcements
            .Where(a => a.BuildingId == building.Id)
            .OrderByDescending(a => a.CreatedAt)
            .Take(3)
            .Select(a => new DashboardVm.AnnouncementItem
            {
                Id = a.Id,
                Title = a.Title,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        // Resident ise: ödenmemiş aidatları göster
        var unpaidFees = new List<DashboardVm.UnpaidFeeItem>();

        if (User.IsInRole("Resident"))
        {
            // 1) Önce DB'den Fee entity'lerini çek (EF burada CalculateAmount görmeyecek)
            var unpaidFeeEntities = await (
                from f in _db.Fees
                join p in _db.FeePayments
                    on new { FeeId = f.Id, UserId = user.Id }
                    equals new { p.FeeId, p.UserId } into pp
                from p in pp.DefaultIfEmpty()
                where f.BuildingId == building.Id
                where p == null || p.IsPaid == false
                orderby f.DueDate
                select f
            ).Take(5).ToListAsync();

            // 2) Sonra C# tarafında hesapla (LINQ-to-Objects)
            unpaidFees = unpaidFeeEntities
                .Select(f => new DashboardVm.UnpaidFeeItem
                {
                    FeeId = f.Id,
                    Title = f.Title,
                    Amount = f.CalculateAmount(),
                    DueDate = f.DueDate
                })
                .ToList();
        }
        var unresolvedFaultCount = await _db.FaultReports
            .CountAsync(x => x.BuildingId == building.Id && !x.IsResolved);

        var latestFaults = await _db.FaultReports
            .Where(x => x.BuildingId == building.Id)
            .OrderBy(x => x.IsResolved)
            .ThenByDescending(x => x.CreatedAt)
            .Take(5)
            .Select(x => new DashboardVm.FaultItem
            {
                Id = x.Id,
                Title = x.Title,
                IsResolved = x.IsResolved,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        var vm = new DashboardVm
        {
            BuildingName = building.Name,
            JoinCode = building.JoinCode,
            LatestAnnouncements = latestAnnouncements,
            UnpaidFees = unpaidFees,
            TotalAnnouncementsCount = totalAnnouncements,
            TotalFeesCount = totalFees,
            UnresolvedFaultCount = unresolvedFaultCount,
            LatestFaults = latestFaults
        };

    

        return View(vm);
    }
}