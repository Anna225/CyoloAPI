using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerAPI.Models;
using Microsoft.AspNetCore.Http.Extensions;

namespace LawyerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtCaseAgendasController : ControllerBase
    {
        private readonly LawyerDbContext _context;

        public CourtCaseAgendasController(LawyerDbContext context)
        {
            _context = context;
        }

        private bool checkURL(string currentUrl)
        {
            if (currentUrl.Contains(".azurewebsites.net"))
            {
                return false;
            }
            return true;
        }

        // GET: api/CourtCaseAgendas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourtCaseAgenda>>> GetCourtCaseAgenda()
        {

            if (checkURL(HttpContext.Request.GetDisplayUrl()))
            {
                return BadRequest();
            }

            return await _context.CourtCaseAgenda.ToListAsync();
        }

        // GET: api/CourtCaseAgendas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourtCaseAgenda>> GetCourtCaseAgenda(int id)
        {

            if (checkURL(HttpContext.Request.GetDisplayUrl()))
            {
                return BadRequest();
            }

            var courtCaseAgenda = await _context.CourtCaseAgenda.FindAsync(id);

            if (courtCaseAgenda == null)
            {
                return NotFound();
            }

            return courtCaseAgenda;
        }

        // PUT: api/CourtCaseAgendas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourtCaseAgenda(int id, CourtCaseAgenda courtCaseAgenda)
        {

            if (checkURL(HttpContext.Request.GetDisplayUrl()))
            {
                return BadRequest();
            }

            if (id != courtCaseAgenda.ID)
            {
                return BadRequest();
            }

            _context.Entry(courtCaseAgenda).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourtCaseAgendaExists(id))
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

        // POST: api/CourtCaseAgendas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CourtCaseAgenda>> PostCourtCaseAgenda(CourtCaseAgenda courtCaseAgenda)
        {

            if (checkURL(HttpContext.Request.GetDisplayUrl()))
            {
                return BadRequest();
            }

            _context.CourtCaseAgenda.Add(courtCaseAgenda);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourtCaseAgenda", new { id = courtCaseAgenda.ID }, courtCaseAgenda);
        }

        // DELETE: api/CourtCaseAgendas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourtCaseAgenda(int id)
        {

            if (checkURL(HttpContext.Request.GetDisplayUrl()))
            {
                return BadRequest();
            }

            var courtCaseAgenda = await _context.CourtCaseAgenda.FindAsync(id);
            if (courtCaseAgenda == null)
            {
                return NotFound();
            }

            _context.CourtCaseAgenda.Remove(courtCaseAgenda);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourtCaseAgendaExists(int id)
        {
            return _context.CourtCaseAgenda.Any(e => e.ID == id);
        }
    }
}
