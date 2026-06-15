using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Infrastructure.Persistence;

namespace SiteManagement.Web.Helpers;

public static class BuildingContext
{
    public static async Task<int?> GetCurrentBuildingIdAsync(
        AppDbContext db,
        UserManager<AppUser> userManager,
        System.Security.Claims.ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return null;

        // Şimdilik: ilk bağlı olduğu building
        var buildingId = await db.BuildingUsers
            .Where(x => x.UserId == user.Id)
            .Select(x => (int?)x.BuildingId)
            .FirstOrDefaultAsync();

        return buildingId;
    }
}