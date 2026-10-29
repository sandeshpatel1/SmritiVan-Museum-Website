using SmritivanWeb;
var builder = WebApplication.CreateBuilder(args);
// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();
// HttpClient for SmritivanApiService (external web service calls)
builder.Services.AddHttpClient<SmritivanApiService>();
// CORS – allow all origins (keeps your existing policy)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
// ── Disable AntiForgery for API endpoints called via fetch() ─────────────────
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
});
var app = builder.Build();
// ── Middleware ────────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
// API controller routes (attribute-routed controllers)
app.MapControllers();
// ── Clean URL routes (no /Home/ prefix) ───────────────────────────────────────
app.MapControllerRoute(
    name: "index",
    pattern: "Index",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "museum",
    pattern: "Museum",
    defaults: new { controller = "Home", action = "Museum" });

app.MapControllerRoute(
    name: "memorial",
    pattern: "Memorial",
    defaults: new { controller = "Home", action = "Memorial" });

app.MapControllerRoute(
    name: "briefhistory",
    pattern: "BriefHistory",
    defaults: new { controller = "Home", action = "BriefHistory" });

app.MapControllerRoute(
    name: "gallery",
    pattern: "Gallery",
    defaults: new { controller = "Home", action = "Gallery" });

app.MapControllerRoute(
    name: "events",
    pattern: "Events",
    defaults: new { controller = "Home", action = "Events" });

app.MapControllerRoute(
    name: "museumtour",
    pattern: "MuseumTour",
    defaults: new { controller = "Home", action = "MuseumTour" });

app.MapControllerRoute(
    name: "amenities",
    pattern: "Amenities",
    defaults: new { controller = "Home", action = "Amenities" });

app.MapControllerRoute(
    name: "mappage",
    pattern: "MapPage",
    defaults: new { controller = "Home", action = "MapPage" });

app.MapControllerRoute(
    name: "faq",
    pattern: "Faq",
    defaults: new { controller = "Home", action = "Faq" });

app.MapControllerRoute(
    name: "contact",
    pattern: "Contact",
    defaults: new { controller = "Home", action = "Contact" });

app.MapControllerRoute(
    name: "terms",
    pattern: "Terms",
    defaults: new { controller = "Home", action = "Terms" });

app.MapControllerRoute(
    name: "privacy",
    pattern: "Privacy",
    defaults: new { controller = "Home", action = "Privacy" });

// ── Default fallback route ────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();