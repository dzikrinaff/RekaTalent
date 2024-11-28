using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RekaTalent.Data;
using NETCore.MailKit.Core;
using static RekaTalent.Data.RekaTalentDbContext;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using RekaTalent.Models;

var builder = WebApplication.CreateBuilder(args);

// Tambahkan konfigurasi CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://example.com")  // Ganti dengan domain yang diizinkan
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();  // Jika Anda memerlukan kredensial (cookie, header otorisasi, dll.)
    });

    // Untuk mengizinkan semua origin (hanya untuk pengujian, tidak direkomendasikan di produksi)
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// Tambahkan konfigurasi DbContext dan Identity
builder.Services.AddDbContext<RekaTalentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<RekaTalentDbContext>()
    .AddDefaultTokenProviders();

// Registrasi EmailService
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Registrasi Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "RekaTalent API", Version = "v1" });
    // Mengonfigurasi enum agar hanya menampilkan angka
    options.MapType<InterviewResult>(() => new OpenApiSchema
    {
        Type = "integer",
        Enum = new List<IOpenApiAny>
        {
            new OpenApiInteger(1),
            new OpenApiInteger(2),
            new OpenApiInteger(3)
        }
    });
});


var app = builder.Build();

// Seed roles dan admin default
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Pastikan role sudah ada di database
    await SeedRoles(roleManager);

    // Pastikan admin default sudah ada
    await SeedAdmin(userManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowSpecificOrigins");

app.Run();

// Fungsi untuk Seed Role
async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    var roles = new[] { "Admin", "Interviewer" };

    foreach (var role in roles)
    {
        var roleExist = await roleManager.RoleExistsAsync(role);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Fungsi untuk Seed Admin Default
async Task SeedAdmin(UserManager<ApplicationUser> userManager)
{
    var adminUser = await userManager.FindByEmailAsync("admin@rektalent.com");

    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = "admin@rektalent.com",
            Email = "admin@rektalent.com"
        };

        var result = await userManager.CreateAsync(newAdmin, "Password@123");

        if (result.Succeeded)
        {
            // Menambahkan role Admin ke user
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}

