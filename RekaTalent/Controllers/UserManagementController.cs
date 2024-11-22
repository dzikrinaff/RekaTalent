using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static RekaTalent.Data.RekaTalentDbContext;

[Route("api/[controller]")]
[ApiController]
public class UserManagementController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagementController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // Mendapatkan daftar pengguna
    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _userManager.Users.Select(u => new
        {
            u.Id,
            u.UserName,
            u.Email
        }).ToList();

        return Ok(users);
    }

    // Menghapus pengguna
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("Pengguna tidak ditemukan");

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
            return Ok("Pengguna berhasil dihapus");

        return BadRequest("Gagal menghapus pengguna");
    }


    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] JsonElement model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("Pengguna tidak ditemukan");

        if (model.TryGetProperty("UserName", out var userNameProp) && userNameProp.ValueKind == JsonValueKind.String)
            user.UserName = userNameProp.GetString();
        if (model.TryGetProperty("Email", out var emailProp) && emailProp.ValueKind == JsonValueKind.String)
            user.Email = emailProp.GetString();

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return Ok(new { message = "Informasi pengguna berhasil diperbarui", userId = user.Id, userName = user.UserName, email = user.Email });
        }

        return BadRequest(new { message = "Gagal memperbarui informasi pengguna", errors = result.Errors.Select(e => e.Description) });
    }


    // Menambahkan role ke pengguna
    [HttpPost("{userId}/add-role")]
    public async Task<IActionResult> AddRoleToUser(string userId, [FromBody] string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("Pengguna tidak ditemukan");

        await _userManager.AddToRoleAsync(user, role);
        return Ok("Role berhasil ditambahkan");
    }
}
