using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("HotelCoreMVCConnection") ?? throw new InvalidOperationException("Connection string 'HotelCoreMVCConnection' not found.");
builder.Services.AddDbContext<HotelCoreMvcContext>(options =>
    options.UseSqlServer(connectionString));
/*builder.Services.AddDatabaseDeveloperPageExceptionFilter();*/
builder.Services.AddControllersWithViews();

// Add authentication dependency injection to authenticate log in user
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/MsUsers/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<HotelCoreMvcContext>();*/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MsUsers}/{action=LogIn}/{id?}");
/*app.MapRazorPages();*/

app.Run();
