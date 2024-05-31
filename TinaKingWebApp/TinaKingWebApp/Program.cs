using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Radzen;
using TinaKingSystem;
using TinaKingSystem.BLL;
using TinaKingWebApp;
using TinaKingWebApp.Areas.Identity;
using TinaKingWebApp.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor.Services;
using TinaKingWebApp.Authentication;
using Syncfusion.Blazor;
using Azure.Storage.Blobs;
using TinaKingSystem.DAL;
using Microsoft.AspNetCore.Identity.UI.Services;
using EmailService;
using Newtonsoft.Json;
using Microsoft.JSInterop;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

// Add services to the DI container
builder.Services.AddAuthenticationCore();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.VisibleStateDuration = 500;
    config.SnackbarConfiguration.ShowCloseIcon = true;
});

// Database and Identity
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionStringTinaKing = builder.Configuration.GetConnectionString("WFS_2590");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<WFS_2590Context>(options => // Make sure this is correctly registered
    options.UseSqlServer(connectionStringTinaKing), ServiceLifetime.Transient);

builder.Services.AddTinaKingDependencies(options => options.UseSqlServer(connectionStringTinaKing));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<EmailSender>();

// Custom and third-party services
builder.Services.AddScoped<ClientInputService>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<UserAccountService>(); // Changed to Scoped to match DbContext lifetime
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<UploadFileService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");



app.Run();
