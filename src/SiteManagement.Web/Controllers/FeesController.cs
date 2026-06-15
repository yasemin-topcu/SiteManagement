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
public class FeesController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public FeesController(AppDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // Herkes: aidatları görür (kendi ödeme durumuyla)
    public async Task<IActionResult> Index()
    {
        var buildingId = await BuildingContext.GetCurrentBuildingIdAsync(_db, _userManager, User);
        if (buildingId == null) return RedirectToAction("Index", "Home");

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var fees = await _db.Fees
            .Where(x => x.BuildingId == buildingId.Value)
            .OrderByDescending(x => x.DueDate)
            .ToListAsync();

        // ödeme durumlarını çek
        var payments = await _db.FeePayments
            .Where(p => fees.Select(f => f.Id).Contains(p.FeeId) && p.UserId == user.Id)
            .ToListAsync();

        // ViewBag ile basit map
        ViewBag.PaymentMap = payments.ToDictionary(p => p.FeeId, p => p);

        return View(fees);
    }

    [Authorize(Roles = "Manager")]
    [HttpGet]
    public IActionResult Create() => View(new FeeCreateVm());

    [Authorize(Roles = "Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FeeCreateVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var buildingId = await BuildingContext.GetCurrentBuildingIdAsync(_db, _userManager, User);
        if (buildingId == null) return RedirectToAction("Index", "Home");

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        Fee fee = vm.FeeType switch
        {
            "Late" => new LateFee(buildingId.Value, vm.Title, vm.Amount, vm.DueDate, user.Id, vm.PenaltyRate ?? 0.10m),
            "Special" => new SpecialFee(buildingId.Value, vm.Title, vm.Amount, vm.DueDate, user.Id, vm.Note),
            _ => new NormalFee(buildingId.Value, vm.Title, vm.Amount, vm.DueDate, user.Id),
        };
        _db.Fees.Add(fee);
        await _db.SaveChangesAsync();

        // Bu apartmandaki tüm Resident'ler için "ödenmemiş" kayıt oluştur
        var residentIds = await _db.BuildingUsers
            .Where(x => x.BuildingId == buildingId.Value && x.MemberType == "Resident")
            .Select(x => x.UserId)
            .ToListAsync();

        foreach (var rid in residentIds)
        {
            _db.FeePayments.Add(new FeePayment(fee.Id, rid));
        }
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    
    // Resident: Ödeme formunu aç
    [Authorize(Roles = "Resident")]
    [HttpGet]
    public async Task<IActionResult> Pay(int feeId)
    {
        var buildingId = await BuildingContext.GetCurrentBuildingIdAsync(_db, _userManager, User);
        if (buildingId == null) return RedirectToAction("Index", "Home");

        var fee = await _db.Fees.FirstOrDefaultAsync(x => x.Id == feeId && x.BuildingId == buildingId.Value);
        if (fee == null) return NotFound();

        ViewBag.FeeTitle = fee.Title;
        ViewBag.Amount = fee.CalculateAmount();
        ViewBag.DueDate = fee.DueDate;

        // 🔹 Special aidat açıklaması
        if (fee is SpecialFee sf && !string.IsNullOrWhiteSpace(sf.Note))
            ViewBag.Note = sf.Note;

        // 🔹 Late aidat gecikme oranı
        if (fee is LateFee lf)
            ViewBag.PenaltyRate = lf.PenaltyRate;

        return View(new PayFeeVm { FeeId = feeId });
    }

    // Resident: Form gönderilince HER KOŞULDA başarılı say
    [Authorize(Roles = "Resident")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(PayFeeVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var buildingId = await BuildingContext.GetCurrentBuildingIdAsync(_db, _userManager, User);
        if (buildingId == null) return RedirectToAction("Index", "Home");

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var fee = await _db.Fees.FirstOrDefaultAsync(x => x.Id == vm.FeeId && x.BuildingId == buildingId.Value);
        if (fee == null) return NotFound();

        var payment = await _db.FeePayments.FirstOrDefaultAsync(x => x.FeeId == vm.FeeId && x.UserId == user.Id);
        if (payment == null)
        {
            payment = new FeePayment(vm.FeeId, user.Id);
            _db.FeePayments.Add(payment);
        }

        // 🔥 Doğrulama YOK: her koşulda öde
        payment.MarkPaid();
        await _db.SaveChangesAsync();

        TempData["Success"] = "Ödeme alındı. (Demo mod: doğrulama yapılmadı)";
        return RedirectToAction(nameof(Index));
    }

    // Manager: Aidat bazlı kim ödedi/ödemedi
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Status(int feeId)
    {
        var buildingId = await BuildingContext.GetCurrentBuildingIdAsync(_db, _userManager, User);
        if (buildingId == null) return RedirectToAction("Index", "Home");

        var fee = await _db.Fees.FirstOrDefaultAsync(x => x.Id == feeId && x.BuildingId == buildingId.Value);
        if (fee == null) return NotFound();

        var rows = await (from bu in _db.BuildingUsers
                          join u in _db.Users on bu.UserId equals u.Id
                          join p in _db.FeePayments on new { FeeId = feeId, UserId = bu.UserId }
                              equals new { p.FeeId, p.UserId } into pp
                          from p in pp.DefaultIfEmpty()
                          where bu.BuildingId == buildingId.Value && bu.MemberType == "Resident"
                          orderby u.FullName
                          select new
                          {
                              u.FullName,
                              u.Email,
                              IsPaid = p != null && p.IsPaid,
                              PaidAt = p!.PaidAt
                          }).ToListAsync();

        ViewBag.FeeTitle = fee.Title;
        ViewBag.Amount = fee.CalculateAmount();
        ViewBag.DueDate = fee.DueDate;
        return View(rows);
    }
}