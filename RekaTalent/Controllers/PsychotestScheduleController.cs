using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RekaTalent.Data;
using RekaTalent.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RekaTalent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PsychotestScheduleController : ControllerBase
    {
        private readonly RekaTalentDbContext _context;

        public PsychotestScheduleController(RekaTalentDbContext context)
        {
            _context = context;
        }

        // GET: api/PsychotestSchedules
        [HttpGet]
        public async Task<ActionResult> GetSchedules()
        {
            var schedules = await _context.PsychotestSchedules
                .Include(ps => ps.Psychotest)  // Mengambil data Psychotest terkait
                .ToListAsync();

            return Ok(schedules);
        }

        // GET: api/PsychotestSchedules/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetSchedule(int id)
        {
            var schedule = await _context.PsychotestSchedules
                .Include(ps => ps.Psychotest)  // Mengambil data Psychotest terkait
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (schedule == null)
            {
                return NotFound("Schedule not found.");
            }

            return Ok(schedule);
        }

        // POST: api/PsychotestSchedules
        [HttpPost]
        public async Task<ActionResult> CreateSchedule([FromBody] PsychotestSchedule request)
        {
            // Cek apakah PsychotestId valid
            var psychotest = await _context.Psychotests.FirstOrDefaultAsync(p => p.Id == request.PsychotestId);

            // Jika PsychotestId tidak ada, buat Psychotest baru
            if (psychotest == null)
            {
                var newPsychotest = new Psychotest
                {
                    // Anda dapat menambahkan logika untuk membuat Psychotest baru, misalnya dari request
                    CandidateId = 1,  // Contoh, Anda bisa menghubungkan dengan ID kandidat yang sesuai
                    CandidateName = "Default Candidate",  // Nama kandidat default
                    Score = 0,  // Nilai default
                    Result = "Pending"  // Status hasil default
                };

                _context.Psychotests.Add(newPsychotest);
                await _context.SaveChangesAsync(); // Simpan Psychotest baru

                // Gunakan Psychotest yang baru dibuat
                psychotest = newPsychotest;
            }

            // Buat entri baru untuk PsychotestSchedule
            var newSchedule = new PsychotestSchedule
            {
                ScheduledDate = request.ScheduledDate,
                PsychotestId = psychotest.Id, // Gunakan ID Psychotest yang valid
                Psychotest = psychotest // Menghubungkan Psychotest dengan PsychotestSchedule
            };

            _context.PsychotestSchedules.Add(newSchedule);
            await _context.SaveChangesAsync();

            // Return data yang berhasil dibuat
            return CreatedAtAction(nameof(GetSchedule), new { id = newSchedule.Id }, newSchedule);
        }

        // PUT: api/PsychotestSchedules/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] PsychotestSchedule request)
        {
            var schedule = await _context.PsychotestSchedules
                .Include(ps => ps.Psychotest)
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (schedule == null)
            {
                return NotFound("Schedule not found.");
            }

            // Update tanggal jadwal
            schedule.ScheduledDate = request.ScheduledDate;

            // Optional: Validasi PsychotestId jika diubah
            if (request.PsychotestId != schedule.PsychotestId)
            {
                var psychotest = await _context.Psychotests.FirstOrDefaultAsync(p => p.Id == request.PsychotestId);
                if (psychotest == null)
                {
                    return BadRequest("Invalid PsychotestId.");
                }
                schedule.PsychotestId = psychotest.Id;
                schedule.Psychotest = psychotest;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PsychotestSchedules/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.PsychotestSchedules
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (schedule == null)
            {
                return NotFound("Schedule not found.");
            }

            _context.PsychotestSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}