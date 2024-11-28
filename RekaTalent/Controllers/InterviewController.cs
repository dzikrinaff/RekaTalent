using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RekaTalent.Models;
using System.Threading.Tasks;
using RekaTalent.Data;

namespace RekaTalent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private readonly RekaTalentDbContext _context;

        public InterviewController(RekaTalentDbContext context)
        {
            _context = context;
        }

        // Endpoint untuk mendapatkan daftar interview
        [HttpGet]
        public async Task<IActionResult> GetInterviews()
        {
            var interviews = await _context.Interviews.ToListAsync();
            return Ok(interviews);
        }

        // Endpoint untuk mendapatkan interview berdasarkan ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInterview(int id)
        {
            var interview = await _context.Interviews.FindAsync(id);
            if (interview == null)
            {
                return NotFound(new { message = "Interview not found." });
            }

            return Ok(interview);
        }

        // Endpoint untuk membuat interview
        [HttpPost]
        [Route("CreateInterview")]
        public async Task<IActionResult> CreateInterview([FromBody] Interview interview)
        {
            // Validasi CandidateId dan ambil data kandidat
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Id == interview.CandidateId);
            if (candidate == null)
            {
                return BadRequest(new { message = "Candidate not found." });
            }

            // Otomatis isi nama dan posisi kandidat
            interview.CandidateName = candidate.Name;
            interview.CandidatePosition = candidate.Position;

            // Simpan interview ke database
            _context.Interviews.Add(interview);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Interview created successfully.",
                interview
            });
        }

        // Endpoint untuk update interview
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInterview(int id, [FromBody] Interview interview)
        {
            var existingInterview = await _context.Interviews.FindAsync(id);
            if (existingInterview == null)
            {
                return NotFound(new { message = "Interview not found." });
            }

            // Update properties interview
            existingInterview.InterviewName = interview.InterviewName;
            existingInterview.CandidateId = interview.CandidateId;
            existingInterview.BackGroundPendidikan = interview.BackGroundPendidikan;
            existingInterview.PengalamanPosisi = interview.PengalamanPosisi;
            existingInterview.ProjectChallenging = interview.ProjectChallenging;
            existingInterview.StrukturOrganisasi = interview.StrukturOrganisasi;
            existingInterview.Pencapaian = interview.Pencapaian;
            existingInterview.FeedBackAtasan = interview.FeedBackAtasan;
            existingInterview.KendalaDeveloper = interview.KendalaDeveloper;
            existingInterview.KelebihanKekurangan = interview.KelebihanKekurangan;
            existingInterview.CurrantSalary = interview.CurrantSalary;
            existingInterview.ExpectedSalary = interview.ExpectedSalary;
            existingInterview.Domisili = interview.Domisili;
            existingInterview.BackGroundDiriKeluarga = interview.BackGroundDiriKeluarga;
            existingInterview.StartDate = interview.StartDate;
            existingInterview.PengetahuanPenilaianDiri = interview.PengetahuanPenilaianDiri;
            existingInterview.PenilaianPnC = interview.PenilaianPnC;
            existingInterview.Result = interview.Result;

            // Simpan perubahan
            await _context.SaveChangesAsync();

            return Ok(new { message = "Interview updated successfully." });
        }

        // Endpoint untuk menghapus interview
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInterview(int id)
        {
            var interview = await _context.Interviews.FindAsync(id);
            if (interview == null)
            {
                return NotFound(new { message = "Interview not found." });
            }

            _context.Interviews.Remove(interview);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Interview deleted successfully." });
        }
    }
}