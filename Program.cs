using Local_Event_Finder.Data;
using Local_Event_Finder.Models;
using Local_Event_Finder.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor Views (for controllers) + Razor Pages (for Identity UI)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Needed for Identity's Razor Pages (Login, Register, etc.)

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

// Location service for distance calculations
builder.Services.AddScoped<ILocationService, LocationService>();

// Event interest service
builder.Services.AddScoped<IEventInterestService, EventInterestService>();

var app = builder.Build();

// Apply migrations & seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.Migrate();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    const string adminRole = "Admin";
    if (!roleMgr.Roles.Any(r => r.Name == adminRole))
        roleMgr.CreateAsync(new IdentityRole(adminRole)).GetAwaiter().GetResult();

    const string adminEmail = "admin@gmail.com";
    var adminUser = userMgr.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        userMgr.CreateAsync(adminUser, "Admin123!").GetAwaiter().GetResult();
    }
    if (!userMgr.IsInRoleAsync(adminUser, adminRole).GetAwaiter().GetResult())
        userMgr.AddToRoleAsync(adminUser, adminRole).GetAwaiter().GetResult();

    if (!db.Events.Any())
    {
        db.Events.AddRange(new[]
        {
            // Ahmedabad Events (Major City - 8 events)
            new Event { Title="Gujarat Tech Summit 2024", Description="Annual technology conference featuring AI, blockchain, and startup innovations.", Category="Tech", City="Ahmedabad", Venue="Gujarat Convention Centre", Address="Sarkhej-Gandhinagar Highway, Ahmedabad, Gujarat 382213", StartUtc=DateTime.UtcNow.AddDays(3), EndUtc=DateTime.UtcNow.AddDays(3).AddHours(8), Organizer="Gujarat IT Association", IsFree=false, TotalSeats=500, AvailableSeats=500, Latitude=23.0225, Longitude=72.5714 },
            new Event { Title="Navratri Garba Night", Description="Traditional Gujarati dance celebration with live music and cultural performances.", Category="Music", City="Ahmedabad", Venue="Sabarmati Riverfront", Address="Sabarmati Riverfront, Ahmedabad, Gujarat 380001", StartUtc=DateTime.UtcNow.AddDays(5), EndUtc=DateTime.UtcNow.AddDays(5).AddHours(4), Organizer="Ahmedabad Cultural Society", IsFree=true, TotalSeats=1000, AvailableSeats=1000, Latitude=23.0225, Longitude=72.5714 },
            new Event { Title="Startup Gujarat Pitch Competition", Description="Early-stage startups compete for funding and mentorship opportunities.", Category="Business", City="Ahmedabad", Venue="IIM Ahmedabad", Address="Vastrapur, Ahmedabad, Gujarat 380015", StartUtc=DateTime.UtcNow.AddDays(7), EndUtc=DateTime.UtcNow.AddDays(7).AddHours(6), Organizer="Gujarat Startup Hub", IsFree=true, TotalSeats=200, AvailableSeats=200, Latitude=23.0225, Longitude=72.5714 },
            new Event { Title="Gujarati Literature Festival", Description="Celebrating Gujarati literature with author talks, book launches, and poetry sessions.", Category="Education", City="Ahmedabad", Venue="Gujarat Vidyapith", Address="Ashram Road, Ahmedabad, Gujarat 380014", StartUtc=DateTime.UtcNow.AddDays(10), EndUtc=DateTime.UtcNow.AddDays(10).AddHours(5), Organizer="Gujarati Sahitya Parishad", IsFree=true, TotalSeats=300, AvailableSeats=300, Latitude=23.0225, Longitude=72.5714 },
            new Event { Title="Ahmedabad Food Festival", Description="Culinary extravaganza featuring traditional Gujarati cuisine and street food.", Category="Food", City="Ahmedabad", Venue="Kankaria Lake", Address="Kankaria Lake, Ahmedabad, Gujarat 380022", StartUtc=DateTime.UtcNow.AddDays(12), EndUtc=DateTime.UtcNow.AddDays(12).AddHours(6), Organizer="Ahmedabad Tourism", IsFree=false, TotalSeats=800, AvailableSeats=800, Latitude=23.0225, Longitude=72.5714 },
            new Event { Title="Gujarat Marathon 2024", Description="Annual marathon promoting fitness and healthy lifestyle in Gujarat.", Category="Sports", City="Ahmedabad", Venue="Sardar Patel Stadium", Address="Motera, Ahmedabad, Gujarat 380005", StartUtc=DateTime.UtcNow.AddDays(15), EndUtc=DateTime.UtcNow.AddDays(15).AddHours(3), Organizer="Gujarat Sports Authority", IsFree=true, TotalSeats=2000, AvailableSeats=2000, Latitude=23.0225, Longitude=72.5714 },
            new Event { Title="Digital Marketing Workshop", Description="Learn modern digital marketing strategies and tools for business growth.", Category="Education", City="Ahmedabad", Venue="CEPT University", Address="Kasturbhai Lalbhai Campus, Ahmedabad, Gujarat 380009", StartUtc=DateTime.UtcNow.AddDays(18), EndUtc=DateTime.UtcNow.AddDays(18).AddHours(4), Organizer="Digital Gujarat", IsFree=false, TotalSeats=100, AvailableSeats=100, Latitude=23.0225, Longitude=72.5714 },
            new Event { Title="Gujarat Art Exhibition", Description="Contemporary and traditional art showcase by local and national artists.", Category="Art", City="Ahmedabad", Venue="Lalbhai Dalpatbhai Museum", Address="University Road, Ahmedabad, Gujarat 380009", StartUtc=DateTime.UtcNow.AddDays(20), EndUtc=DateTime.UtcNow.AddDays(20).AddHours(7), Organizer="Gujarat Lalit Kala Academy", IsFree=true, TotalSeats=400, AvailableSeats=400, Latitude=23.0225, Longitude=72.5714 },

            // Surat Events (Major City - 6 events)
            new Event { Title="Diamond Industry Conference", Description="Global diamond trade and technology innovations in Surat's diamond hub.", Category="Business", City="Surat", Venue="Surat International Exhibition Centre", Address="Dumas Road, Surat, Gujarat 395007", StartUtc=DateTime.UtcNow.AddDays(4), EndUtc=DateTime.UtcNow.AddDays(4).AddHours(6), Organizer="Surat Diamond Association", IsFree=false, TotalSeats=300, AvailableSeats=300, Latitude=21.1702, Longitude=72.8311 },
            new Event { Title="Garba Dance Workshop", Description="Learn traditional Garba dance steps and techniques from expert instructors.", Category="Music", City="Surat", Venue="Surat Municipal Corporation Hall", Address="Athwa Lines, Surat, Gujarat 395001", StartUtc=DateTime.UtcNow.AddDays(8), EndUtc=DateTime.UtcNow.AddDays(8).AddHours(3), Organizer="Surat Cultural Centre", IsFree=true, TotalSeats=150, AvailableSeats=150, Latitude=21.1702, Longitude=72.8311 },
            new Event { Title="Textile Innovation Summit", Description="Latest trends in textile manufacturing and sustainable fashion.", Category="Business", City="Surat", Venue="Surat Textile Market", Address="Ring Road, Surat, Gujarat 395002", StartUtc=DateTime.UtcNow.AddDays(11), EndUtc=DateTime.UtcNow.AddDays(11).AddHours(5), Organizer="Surat Textile Association", IsFree=false, TotalSeats=250, AvailableSeats=250, Latitude=21.1702, Longitude=72.8311 },
            new Event { Title="Gujarati Folk Music Concert", Description="Traditional folk music performance by renowned Gujarati artists.", Category="Music", City="Surat", Venue="Gopi Talav", Address="Gopi Talav, Surat, Gujarat 395003", StartUtc=DateTime.UtcNow.AddDays(14), EndUtc=DateTime.UtcNow.AddDays(14).AddHours(3), Organizer="Surat Music Society", IsFree=true, TotalSeats=500, AvailableSeats=500, Latitude=21.1702, Longitude=72.8311 },
            new Event { Title="Surat Food Walk", Description="Guided food tour exploring Surat's famous street food and local delicacies.", Category="Food", City="Surat", Venue="Surat City Centre", Address="City Light Area, Surat, Gujarat 395007", StartUtc=DateTime.UtcNow.AddDays(17), EndUtc=DateTime.UtcNow.AddDays(17).AddHours(4), Organizer="Surat Tourism Board", IsFree=false, TotalSeats=50, AvailableSeats=50, Latitude=21.1702, Longitude=72.8311 },
            new Event { Title="Tech Startup Meetup", Description="Networking event for tech entrepreneurs and startup founders in Surat.", Category="Tech", City="Surat", Venue="Surat IT Hub", Address="Piplod, Surat, Gujarat 395007", StartUtc=DateTime.UtcNow.AddDays(21), EndUtc=DateTime.UtcNow.AddDays(21).AddHours(3), Organizer="Surat Tech Community", IsFree=true, TotalSeats=120, AvailableSeats=120, Latitude=21.1702, Longitude=72.8311 },

            // Vadodara Events (Major City - 4 events)
            new Event { Title="MSU Cultural Festival", Description="Annual cultural festival at MSU featuring music, dance, and drama performances.", Category="Music", City="Vadodara", Venue="MSU Campus", Address="Fatehgunj, Vadodara, Gujarat 390002", StartUtc=DateTime.UtcNow.AddDays(6), EndUtc=DateTime.UtcNow.AddDays(6).AddHours(5), Organizer="MSU Cultural Committee", IsFree=true, TotalSeats=600, AvailableSeats=600, Latitude=22.3072, Longitude=73.1812 },
            new Event { Title="Gujarat Science Fair", Description="Science exhibition showcasing innovations and research from Gujarat universities.", Category="Education", City="Vadodara", Venue="Science City", Address="Science City, Vadodara, Gujarat 390002", StartUtc=DateTime.UtcNow.AddDays(9), EndUtc=DateTime.UtcNow.AddDays(9).AddHours(6), Organizer="Gujarat Science Council", IsFree=true, TotalSeats=400, AvailableSeats=400, Latitude=22.3072, Longitude=73.1812 },
            new Event { Title="Vadodara Heritage Walk", Description="Guided tour of Vadodara's historical monuments and architectural heritage.", Category="Education", City="Vadodara", Venue="Laxmi Vilas Palace", Address="J N Marg, Vadodara, Gujarat 390001", StartUtc=DateTime.UtcNow.AddDays(13), EndUtc=DateTime.UtcNow.AddDays(13).AddHours(3), Organizer="Vadodara Heritage Trust", IsFree=false, TotalSeats=80, AvailableSeats=80, Latitude=22.3072, Longitude=73.1812 },
            new Event { Title="Gujarati Theatre Festival", Description="Traditional and contemporary Gujarati plays and theatrical performances.", Category="Art", City="Vadodara", Venue="Natak Mandir", Address="Raopura, Vadodara, Gujarat 390001", StartUtc=DateTime.UtcNow.AddDays(16), EndUtc=DateTime.UtcNow.AddDays(16).AddHours(4), Organizer="Vadodara Theatre Group", IsFree=false, TotalSeats=200, AvailableSeats=200, Latitude=22.3072, Longitude=73.1812 },

            // Rajkot Events (Major City - 3 events)
            new Event { Title="Rajkot Business Summit", Description="Annual business conference focusing on Saurashtra's economic development.", Category="Business", City="Rajkot", Venue="Rajkot Chamber of Commerce", Address="Kalavad Road, Rajkot, Gujarat 360005", StartUtc=DateTime.UtcNow.AddDays(7), EndUtc=DateTime.UtcNow.AddDays(7).AddHours(5), Organizer="Rajkot Chamber of Commerce", IsFree=false, TotalSeats=200, AvailableSeats=200, Latitude=22.3039, Longitude=70.8022 },
            new Event { Title="Saurashtra Cricket Tournament", Description="Regional cricket championship featuring local teams and players.", Category="Sports", City="Rajkot", Venue="Saurashtra Cricket Stadium", Address="Khandheri, Rajkot, Gujarat 360022", StartUtc=DateTime.UtcNow.AddDays(19), EndUtc=DateTime.UtcNow.AddDays(19).AddHours(8), Organizer="Saurashtra Cricket Association", IsFree=true, TotalSeats=1500, AvailableSeats=1500, Latitude=22.3039, Longitude=70.8022 },
            new Event { Title="Gujarati Poetry Recitation", Description="Evening of Gujarati poetry with renowned poets and literary figures.", Category="Education", City="Rajkot", Venue="Rajkot Public Library", Address="Gandhi Road, Rajkot, Gujarat 360001", StartUtc=DateTime.UtcNow.AddDays(22), EndUtc=DateTime.UtcNow.AddDays(22).AddHours(3), Organizer="Rajkot Literary Society", IsFree=true, TotalSeats=150, AvailableSeats=150, Latitude=22.3039, Longitude=70.8022 },

            // Gandhinagar Events (Capital City - 2 events)
            new Event { Title="Gujarat Government Tech Expo", Description="Showcasing government technology initiatives and digital governance.", Category="Tech", City="Gandhinagar", Venue="Gujarat Secretariat", Address="Sector 10, Gandhinagar, Gujarat 382010", StartUtc=DateTime.UtcNow.AddDays(5), EndUtc=DateTime.UtcNow.AddDays(5).AddHours(6), Organizer="Gujarat IT Department", IsFree=true, TotalSeats=300, AvailableSeats=300, Latitude=23.2156, Longitude=72.6369 },
            new Event { Title="Gujarat Day Celebration", Description="Annual celebration of Gujarat's formation with cultural programs and exhibitions.", Category="Music", City="Gandhinagar", Venue="Gujarat Vidhan Sabha", Address="Sector 10, Gandhinagar, Gujarat 382010", StartUtc=DateTime.UtcNow.AddDays(1), EndUtc=DateTime.UtcNow.AddDays(1).AddHours(5), Organizer="Gujarat Government", IsFree=true, TotalSeats=1000, AvailableSeats=1000, Latitude=23.2156, Longitude=72.6369 },

            // Bhavnagar Events (Coastal City - 1 event)
            new Event { Title="Bhavnagar Beach Festival", Description="Coastal celebration with water sports, beach games, and cultural performances.", Category="Sports", City="Bhavnagar", Venue="Ghogha Beach", Address="Ghogha Beach, Bhavnagar, Gujarat 364001", StartUtc=DateTime.UtcNow.AddDays(8), EndUtc=DateTime.UtcNow.AddDays(8).AddHours(6), Organizer="Bhavnagar Tourism", IsFree=true, TotalSeats=800, AvailableSeats=800, Latitude=21.7645, Longitude=72.1519 },

            // Jamnagar Events (Coastal City - 1 event)
            new Event { Title="Marine Biology Workshop", Description="Educational workshop on marine life and coastal ecosystem conservation.", Category="Education", City="Jamnagar", Venue="Marine National Park", Address="Marine National Park, Jamnagar, Gujarat 361140", StartUtc=DateTime.UtcNow.AddDays(12), EndUtc=DateTime.UtcNow.AddDays(12).AddHours(4), Organizer="Gujarat Forest Department", IsFree=false, TotalSeats=60, AvailableSeats=60, Latitude=22.4707, Longitude=70.0577 }
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

// Identity UI (Razor Pages)
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
