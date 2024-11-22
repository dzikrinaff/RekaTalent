using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RekaTalent.Data;
using RekaTalent.Models;
using System.IO;
using System.Threading.Tasks;

namespace RekaTalent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly RekaTalentDbContext _context;

        public FileController(RekaTalentDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File tidak ditemukan");

            // Membaca file sebagai byte array
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // Menyimpan ke database
            var newFile = new FileModel
            {
                FileName = file.FileName,
                FileData = fileBytes,
                ContentType = file.ContentType
            };

            _context.Files.Add(newFile);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "File berhasil diunggah ke database" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            var file = await _context.Files.FindAsync(id);

            if (file == null)
                return NotFound("File tidak ditemukan");

            return File(file.FileData, file.ContentType, file.FileName);
        }

        [HttpGet]
        public IActionResult GetAllFiles()
        {
            var files = _context.Files.Select(f => new
            {
                f.Id,
                f.FileName,
                f.ContentType
            }).ToList();

            return Ok(files);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var file = await _context.Files.FindAsync(id);

            if (file == null)
                return NotFound("File tidak ditemukan");

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "File berhasil dihapus" });
        }
    }
}
