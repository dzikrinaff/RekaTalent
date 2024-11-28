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
    public class PsychotestController : ControllerBase
    {
        private readonly RekaTalentDbContext _context;

        public PsychotestController(RekaTalentDbContext context)
        {
            _context = context;
        }

        // GET: api/Psychotests
        [HttpGet]
        public async Task<ActionResult> GetPsychotests()
        {
            var psychotests = await _context.Psychotests.ToListAsync();
            return Ok(psychotests);
        }

        // GET: api/Psychotests/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetPsychotest(int id)
        {
            var psychotest = await _context.Psychotests.FirstOrDefaultAsync(pt => pt.Id == id);

            if (psychotest == null)
            {
                return NotFound("Psychotest not found.");
            }

            return Ok(psychotest);
        }

        // POST: api/Psychotests
        [HttpPost]
        public async Task<ActionResult> CreatePsychotest([FromBody] Psychotest request)
        {
            // Validasi apakah CandidateId valid
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Id == request.CandidateId);
            if (candidate == null)
            {
                return BadRequest("Candidate not found.");
            }

            // Buat entri baru Psychotest
            var newPsychotest = new Psychotest
            {
                CandidateId = candidate.Id,
                CandidateName = candidate.Name,
                Score = request.Score,
                Result = request.Score >= 75 ? "Passed" : "Not Passed"
            };

            _context.Psychotests.Add(newPsychotest);
            await _context.SaveChangesAsync();

            // Return data yang berhasil dibuat
            return CreatedAtAction(nameof(GetPsychotest), new { id = newPsychotest.Id }, newPsychotest);
        }

        // PUT: api/Psychotests/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePsychotest(int id, [FromBody] Psychotest request)
        {
            var psychotest = await _context.Psychotests.FirstOrDefaultAsync(pt => pt.Id == id);

            if (psychotest == null)
            {
                return NotFound("Psychotest not found.");
            }

            // Update data
            psychotest.Score = request.Score;
            psychotest.Result = request.Score >= 75 ? "Passed" : "Not Passed";

            // Optional: Validasi CandidateId jika ingin diubah
            if (request.CandidateId != psychotest.CandidateId)
            {
                var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Id == request.CandidateId);
                if (candidate == null)
                {
                    return BadRequest("Candidate not found.");
                }
                psychotest.CandidateId = candidate.Id;
                psychotest.CandidateName = candidate.Name;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Psychotests/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePsychotest(int id)
        {
            var psychotest = await _context.Psychotests.FirstOrDefaultAsync(pt => pt.Id == id);

            if (psychotest == null)
            {
                return NotFound("Psychotest not found.");
            }

            _context.Psychotests.Remove(psychotest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}