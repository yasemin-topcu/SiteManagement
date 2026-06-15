using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace SiteManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        // Rol isimleri
        var roleName = vm.UserType == "Manager" ? "Manager" : "Resident";

        // Roller yoksa oluştur
        if (!await _roleManager.RoleExistsAsync("Manager"))
            await _roleManager.CreateAsync(new IdentityRole("Manager"));
        if (!await _roleManager.RoleExistsAsync("Resident"))
            await _roleManager.CreateAsync(new IdentityRole("Resident"));

        // User oluştur
        var user = new AppUser
        {
            UserName = vm.Email,
            Email = vm.Email,
            PhoneNumber = vm.PhoneNumber,
            FullName = vm.FullName,
            UserType = vm.UserType
        };

        var result = await _userManager.CreateAsync(user, vm.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors)
                ModelState.AddModelError("", e.Description);
            return View(vm);
        }

        await _userManager.AddToRoleAsync(user, roleName);

        // Resident ise JoinCode zorunlu + apartmana bağla
        if (vm.UserType == "Resident")
        {
            if (string.IsNullOrWhiteSpace(vm.JoinCode))
            {
                ModelState.AddModelError(nameof(vm.JoinCode), "Katılım kodu zorunludur.");
                return View(vm);
            }

            // JoinCode ile apartmanı bul
            var db = HttpContext.RequestServices.GetRequiredService<SiteManagement.Infrastructure.Persistence.AppDbContext>();
            var building = await db.Buildings.FirstOrDefaultAsync(x => x.JoinCode == vm.JoinCode.Trim());
            if (building == null)
            {
                ModelState.AddModelError(nameof(vm.JoinCode), "Katılım kodu hatalı.");
                return View(vm);
            }

            // ilişki kaydı
            var link = new SiteManagement.Domain.Entities.BuildingUser(building.Id, user.Id, "Resident");
            db.BuildingUsers.Add(link);
            await db.SaveChangesAsync();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Giriş başarısız. E-posta veya şifre hatalı.");
            return View(vm);
        }

        if (!string.IsNullOrWhiteSpace(returnUrl)) return LocalRedirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}