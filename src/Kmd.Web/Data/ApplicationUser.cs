using Microsoft.AspNetCore.Identity;

namespace Kmd.Web.Data;

public class ApplicationUser : IdentityUser
{
    public string? ClassName { get; set; }
    public string? OtherContact { get; set; }
}
