using Local_Event_Finder.Data;
using Local_Event_Finder.Models;
using Local_Event_Finder.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor Views
builder.Services.AddControllersWithViews();

// Database (SQL Server)
var conn = builder.Configuration.GetConnectionString("AppDb");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(conn));

// Identity (users + roles)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<AppDbContext>()
  .AddDefaultTokenProviders()
  .AddDefaultUI();

builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Identity/Account/Login"; // identity default path
    o.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Event service (DB backed)
builder.Services.AddScoped<IEventService, DbEventService>();

var app = builder.Build();

// Apply migrations & seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Now that we have an initial migration, always use Migrate() so __EFMigrationsHistory stays consistent.
    db.Database.Migrate();

    // Seed roles + admin (idempotent)
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    const string adminRole = "Admin";
    if (!roleMgr.Roles.Any(r => r.Name == adminRole))
        roleMgr.CreateAsync(new IdentityRole(adminRole)).GetAwaiter().GetResult();

    const string adminEmail = "admin@local.test";
    var adminUser = userMgr.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        userMgr.CreateAsync(adminUser, "Admin123!").GetAwaiter().GetResult();
    }
    if (!userMgr.IsInRoleAsync(adminUser, adminRole).GetAwaiter().GetResult())
        userMgr.AddToRoleAsync(adminUser, adminRole).GetAwaiter().GetResult();

    // Seed sample events only if table empty
    if (!db.Events.Any())
    {
        db.Events.AddRange(new[]
        {
            new Event { Title="Intro Tech Meetup", Description="Kickoff session for tech enthusiasts.", Category="Tech", City="Seattle", Venue="Community Hub", StartUtc=DateTime.UtcNow.AddDays(2), EndUtc=DateTime.UtcNow.AddDays(2).AddHours(2), Organizer="CoreTeam", IsFree=true },
            new Event { Title="Evening Jazz", Description="Smooth jazz performances.", Category="Music", City="Seattle", Venue="Blue Note Hall", StartUtc=DateTime.UtcNow.AddDays(4), EndUtc=DateTime.UtcNow.AddDays(4).AddHours(3), Organizer="CityArts", IsFree=false },
            new Event { Title="Startup Pitch Night", Description="Founders pitch early ideas.", Category="Business", City="Portland", Venue="Innovation Loft", StartUtc=DateTime.UtcNow.AddDays(6), EndUtc=DateTime.UtcNow.AddDays(6).AddHours(1), Organizer="StartupOrg", IsFree=true }
        });
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
