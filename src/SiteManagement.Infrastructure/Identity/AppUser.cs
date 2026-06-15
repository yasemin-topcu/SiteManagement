using Microsoft.AspNetCore.Identity;

namespace SiteManagement.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = default!;
    public string UserType { get; set; } = default!; // "Manager" | "Resident"
}
