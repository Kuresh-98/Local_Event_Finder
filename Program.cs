using Local_Event_Finder.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// Revert to simple in-memory service only
builder.Services.AddSingleton<IEventService, InMemoryEventService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
