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
    public class InterviewScheduleController : ControllerBase
    {
        private readonly RekaTalentDbContext _context;

        public InterviewScheduleController(RekaTalentDbContext context)
        {
            _context = context;
        }

        // GET: api/InterviewSchedules
        [HttpGet]
        public async Task<ActionResult> GetInterviewSchedules()
        {
            var schedules = await _context.InterviewSchedules.Include(schedule => schedule.Interview).ToListAsync();
            return Ok(schedules);
        }

        // GET: api/InterviewSchedules/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetInterviewSchedule(int id)
        {
            var schedule = await _context.InterviewSchedules
                .Include(schedule => schedule.Interview)
                .FirstOrDefaultAsync(schedule => schedule.Id == id);

            if (schedule == null)
            {
                return NotFound("Interview schedule not found.");
            }

            return Ok(schedule);
        }

        // POST: api/InterviewSchedules
        [HttpPost]
        public async Task<ActionResult> CreateInterviewSchedule([FromBody] InterviewSchedule request)
        {
            // Cek apakah InterviewId valid
            var interview = await _context.Interviews.FirstOrDefaultAsync(i => i.Id == request.InterviewId);

            // Jika InterviewId tidak ada, maka buat Interview baru
            if (interview == null)
            {
                // Buat Interview baru dengan data dasar (contoh, dapat disesuaikan sesuai kebutuhan)
                var newInterview = new Interview
                {
                    InterviewName = "Default Interview", // Atur nama interview sesuai kebutuhan
                    CandidateId = 1, // Anda bisa menyesuaikan dengan candidate yang relevan, misalnya dari frontend
                    CandidateName = "Default Candidate", // Atur nama kandidat
                    CandidatePosition = "Developer", // Atur posisi kandidat
                    Result = InterviewResult.Pending // Status interview, bisa disesuaikan
                };

                _context.Interviews.Add(newInterview);
                await _context.SaveChangesAsync(); // Simpan interview baru

                // Ambil interview yang baru saja dibuat
                interview = newInterview;
            }

            // Buat entri baru InterviewSchedule dan kaitkan dengan Interview yang sudah ada
            var newSchedule = new InterviewSchedule
            {
                ScheduledDate = request.ScheduledDate,
                InterviewId = interview.Id, // Gunakan ID interview yang valid
                Interview = interview // Menghubungkan Interview dengan InterviewSchedule
            };

            _context.InterviewSchedules.Add(newSchedule);
            await _context.SaveChangesAsync();

            // Return data yang berhasil dibuat
            return CreatedAtAction(nameof(GetInterviewSchedule), new { id = newSchedule.Id }, newSchedule);
        }

        // PUT: api/InterviewSchedules/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInterviewSchedule(int id, [FromBody] InterviewSchedule request)
        {
            var schedule = await _context.InterviewSchedules
                .Include(schedule => schedule.Interview)
                .FirstOrDefaultAsync(schedule => schedule.Id == id);

            if (schedule == null)
            {
                return NotFound("Interview schedule not found.");
            }

            // Update tanggal jadwal
            schedule.ScheduledDate = request.ScheduledDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/InterviewSchedules/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInterviewSchedule(int id)
        {
            var schedule = await _context.InterviewSchedules
                .Include(schedule => schedule.Interview)
                .FirstOrDefaultAsync(schedule => schedule.Id == id);

            if (schedule == null)
            {
                return NotFound("Interview schedule not found.");
            }

            _context.InterviewSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}