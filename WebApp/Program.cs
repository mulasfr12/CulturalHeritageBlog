using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.Services.Interfaces;
using WebAPI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CulturalHeritageDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           );


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICulturalHeritageService, CulturalHeritageService>();
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ILogServices, LogService>();
builder.Services.AddScoped<INationalMinorityService, NationalMinorityService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
