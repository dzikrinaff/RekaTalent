using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RekaTalent.Data;
using RekaTalent.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RekaTalent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly RekaTalentDbContext _context;

        public CandidateController(RekaTalentDbContext context)
        {
            _context = context;
        }

        // GET: api/Candidate
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
        {
            var candidates = await _context.Candidates.ToListAsync();
            return Ok(candidates);
        }

        // GET: api/Candidate/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Candidate>> GetCandidate(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);

            if (candidate == null)
            {
                return NotFound("Candidate not found");
            }

            return Ok(candidate);
        }

        // POST: api/Candidate
        [HttpPost]
        public async Task<ActionResult<Candidate>> CreateCandidate(Candidate candidate)
        {
            if (candidate == null)
            {
                return BadRequest("Candidate data is invalid");
            }

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCandidate), new { id = candidate.Id }, candidate);
        }

        // PUT: api/Candidate/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCandidate(int id, Candidate candidate)
        {
            if (id != candidate.Id)
            {
                return BadRequest("Candidate ID mismatch");
            }

            var existingCandidate = await _context.Candidates.FindAsync(id);

            if (existingCandidate == null)
            {
                return NotFound("Candidate not found");
            }

            // Update properties
            existingCandidate.Name = candidate.Name;
            existingCandidate.Position = candidate.Position;
            existingCandidate.Email = candidate.Email;
            existingCandidate.PhoneNumber = candidate.PhoneNumber;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Candidate/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);

            if (candidate == null)
            {
                return NotFound("Candidate not found");
            }

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}