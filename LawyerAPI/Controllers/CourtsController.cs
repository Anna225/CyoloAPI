using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerAPI.Models;
using LawyerAPI.Helper;

namespace LawyerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtsController : ControllerBase
    {
        private readonly LawyerDbContext _context;

        public CourtsController(LawyerDbContext context)
        {
            _context = context;
        }
        
        private bool CheckHeaderData(string headerKey)
        {
            HttpContext.Request.Headers.TryGetValue(headerKey, out var headerValue);
            if (headerValue == "d23d9c7c11da4b228417e567c85fa80c")
            {
                return true;
            }
            return false;
        }

        private bool checkURL(string currentUrl)
        {
            if (currentUrl.Contains(".azurewebsites.net"))
            {
                return false;
            }
            return true;
        }

        // GET: api/Courts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Court>>> GetCourts()
        {

          if (_context.Courts == null)
          {
              return NotFound();
          }
            return await _context.Courts.ToListAsync();
        }

        // GET: api/Courts/JuridictionTypes
        [HttpGet("JuridictionTypes")]
        public async Task<ActionResult<IEnumerable<JuridictionTypeDto>>> JuridictionTypes()
        {
            if (_context.Courts == null)
            {
                return NotFound();
            }
            return await _context.Courts.Select(x => new JuridictionTypeDto
            {
                Canton = x.Canton,
                Division = x.Division,
                TypeJuridiction = x.TypeJuridiction,
                TypeJuridictionId = x.TypeJuridictionId,
                DivisionId = x.DivisionId
            })
            .OrderBy(p => p.TypeJuridiction)
            .ToListAsync();
        }

        // GET: api/Courts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Court>> GetCourt(int id)
        {

          if (_context.Courts == null)
          {
              return NotFound();
          }
            var court = await _context.Courts.FindAsync(id);

            if (court == null)
            {
                return NotFound();
            }

            return court;
        }

        // PUT: api/Courts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourt(int id, Court court)
        {

            if (id != court.ID)
            {
                return BadRequest();
            }

            _context.Entry(court).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourtExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Courts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Court>> PostCourt(Court court)
        {

          if (_context.Courts == null)
          {
              return Problem("Entity set 'LawyerDbContext.Courts'  is null.");
          }
            _context.Courts.Add(court);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourt), new { id = court.ID }, court);
        }

        // DELETE: api/Courts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourt(int id)
        {

            if (_context.Courts == null)
            {
                return NotFound();
            }
            var court = await _context.Courts.FindAsync(id);
            if (court == null)
            {
                return NotFound();
            }

            _context.Courts.Remove(court);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourtExists(long id)
        {
            return (_context.Courts?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
