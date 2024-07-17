using Microsoft.EntityFrameworkCore;
using MultiTenantApp;
using MultiTenantApp.DbContexts;
using MultiTenantApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication("Bearer").AddIdentityServerAuthentication("Bearer", options =>
{
    options.ApiName = "myApi";
    options.Authority = "https://localhost:7288";
});

// Multitenancy
builder.Services.AddMultitenancy<Tenants, TenantResolver>();

// survice registration
builder.Services.AddTransient<ITenantService, TenantService>();
builder.Services.AddTransient<IAppUserService, AppUserService>();

// Sql Server TenantDb Connection
builder.Services.AddDbContextPool<TenantDbContext>(options => options.
        UseSqlServer(builder.Configuration.GetConnectionString("TenantConnection")));
builder.Services.AddDbContext<AppDbContext>();

// Session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

// Multitenancy
app.UseMultitenancy<Tenants>();

//Use Session
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Signin}/{id?}");

app.Run();
