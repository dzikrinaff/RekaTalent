using RekaTalent.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using static RekaTalent.Data.RekaTalentDbContext;
using NETCore.MailKit.Core;


var builder = WebApplication.CreateBuilder(args);

// Tambahkan konfigurasi DbContext dan Identity
builder.Services.AddDbContext<RekaTalentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<RekaTalentDbContext>()
    .AddDefaultTokenProviders();

// Registrasi EmailService
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


// Fungsi untuk Seed Role dan Admin Default
async Task SeedRolesAndAdmin(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
{
    string[] roles = { "Admin", "Interviewer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role)) // Cek apakah role sudah ada
        {
            await roleManager.CreateAsync(new IdentityRole(role)); // Buat role jika belum ada
        }
    }


    // Buat pengguna Admin default
    var adminEmail = "admin@rekatalent.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser { UserName = "admin", Email = adminEmail, EmailConfirmed = true };
        var result = await userManager.CreateAsync(newAdmin, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}
