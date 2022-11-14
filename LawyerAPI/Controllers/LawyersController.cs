using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerAPI.Models;
using Microsoft.AspNetCore.Http.Extensions;
using LawyerAPI.Helper;

namespace LawyerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LawyersController : ControllerBase
    {
        private readonly LawyerDbContext _context;

        private bool checkURL(string currentUrl)
        {
            if (currentUrl.Contains(".azurewebsites.net"))
            {
                return false;
            }
            return true;
        }

        public LawyersController(LawyerDbContext context)
        {
            
            _context = context;
        }
        
        // GET: api/Lawyers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lawyer>>> GetLawyers()
        {
          if (_context.Lawyers == null)
          {
              return NotFound();
          }
          return await _context.Lawyers.ToListAsync();
        }

        // GET: api/Lawyer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lawyer>> GetLawyer(int id)
        {
            if (_context.Lawyers == null)
            {
                return NotFound();
            }
            var lawyer = await _context.Lawyers.FindAsync(id);

            if (lawyer == null)
            {
                return NotFound();
            }

            return lawyer;
        }

        // GET: api/Lawyer/5
        [HttpGet("ByEmail/{email}")]
        public async Task<ActionResult<Lawyer>> GetLawyerByEmail(string email)
        {
            if (_context.Lawyers == null)
            {
                return NotFound();
            }
            var lawyer = await _context.Lawyers.Where(x => x.Email!.Contains(email)).FirstOrDefaultAsync();

            if (lawyer == null)
            {
                return NotFound();
            }

            return lawyer;
        }

        // POST: api/Lawyer/5
        [HttpPost("GetByCourtCase")]
        public async Task<ActionResult<dynamic>> GetByCourtCase(SearchDto condition)
        {
            if (condition.ChamberID == null || condition.ChamberID == "")
            {
                var lawyers = (from present in _context.Presentations
                               join lawyer in _context.Lawyers
                               on new { id = present.LawyerId, val = present.Available }
                               equals new { id = lawyer.ID, val = 1 }
                               join courtcase in _context.CourtCaseAgenda
                               on new { courtno = present.CourtCaseNo }
                               equals new { courtno = courtcase.CourtCaseNo }
                               where courtcase.HearingGeneral == condition.HearingGeneral
                               && (courtcase.HearingDate == condition.HearingDate)
                               && (courtcase.HearingTime == condition.HearingTime)
                               select new { lawyer }).Distinct();

                if (lawyers == null)
                {
                    return NotFound();
                }
                return await lawyers.ToListAsync();
            }
            else
            {
                var lawyers = (from present in _context.Presentations
                               join lawyer in _context.Lawyers
                               on new { id = present.LawyerId, val = present.Available }
                               equals new { id = lawyer.ID, val = 1 }
                               join courtcase in _context.CourtCaseAgenda
                               on new { courtno = present.CourtCaseNo }
                               equals new { courtno = courtcase.CourtCaseNo }
                               where courtcase.HearingGeneral == condition.HearingGeneral
                               && (courtcase.HearingDate == condition.HearingDate)
                               && (courtcase.HearingTime == condition.HearingTime)
                               && (courtcase.ChamberID == condition.ChamberID)
                               select new { lawyer }).Distinct();

                if (lawyers == null)
                {
                    return NotFound();
                }
                return await lawyers.ToListAsync();
            }

            return NotFound();

        }

        // PUT: api/Lawyer/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLawyer(int id, Lawyer lawyer)
        {
            if (id != lawyer.ID)
            {
                return BadRequest();
            }

            _context.Entry(lawyer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LawyersExists(id))
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

        // POST: api/Lawyer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Lawyer>> PostLawyer(Lawyer lawyer)
        {

          if (_context.Lawyers == null)
          {
              return Problem("Entity set 'LawyerDbContext.Courts'  is null.");
          }
            _context.Lawyers.Add(lawyer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLawyer), new { ID = lawyer.ID }, lawyer);
        }

        // DELETE: api/Lawyer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourt(int id)
        {
            if (_context.Lawyers == null)
            {
                return NotFound();
            }
            var lawyer = await _context.Lawyers.FindAsync(id);
            if (lawyer == null)
            {
                return NotFound();
            }

            _context.Lawyers.Remove(lawyer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LawyersExists(int id)
        {
            return (_context.Lawyers?.Any(e => e.ID == id)).GetValueOrDefault();
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
    }
}
