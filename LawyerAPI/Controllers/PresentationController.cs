using LawyerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawyerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresentationController : ControllerBase
    {
        private readonly LawyerDbContext _context;

        public PresentationController(LawyerDbContext context)
        {
            _context = context;
        }

        // GET: api/Presentations/2021-1155-A
        [HttpGet("GetByCourtCaseNo/{courtcaseno}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAvailableLawyersByCourtCaseNo(string courtcaseno)
        {
            var lawyers = (from presentation in _context.Presentations
                           join lawyer in _context.Lawyers
                           on new { ID = presentation.LawyerId }
                           equals new { ID = lawyer.ID }
                           where presentation.CourtCaseNo!.Contains(courtcaseno) &&
                                   (presentation.Available == 1)
                           select new { presentation, lawyer }).Distinct();

            if (lawyers == null)
            {
                return NotFound();
            }
            return await lawyers.ToListAsync();

        }

        // POST: api/Presentations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Presentation>> Presentation(Presentation presentation)
        {

            if (_context.Presentations == null)
            {
                return Problem("Entity set 'LawyerDbContext.Presentations'  is null.");
            }
            _context.Presentations.Add(presentation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPresentation), new { id = presentation.ID }, presentation);
        }

        // GET: api/Presentations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Presentation>> GetPresentation(int id)
        {

            if (_context.Presentations == null)
            {
                return NotFound();
            }
            var presentation = await _context.Presentations.FindAsync(id);

            if (presentation == null)
            {
                return NotFound();
            }

            return presentation;
        }

        private string CourtExists(string courtcaseno)
        {
            return _context.Presentations.Any(e => e.CourtCaseNo!.Contains(courtcaseno)).ToString();
        }
    }
}
