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
    public class AgendaController : ControllerBase
    {
        private readonly LawyerDbContext _context;

        public AgendaController(LawyerDbContext context)
        {
            _context = context;
        }

        // POST: api/Agendas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Agenda>> CreateAgenda(Agenda agenda)
        {
            _context.Agendas.Add(agenda);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAgenda", new { id = agenda.ID }, agenda);
        }

        // GET: api/Agendas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Agenda>> GetAgenda(int id)
        {
            var agenda = await _context.Agendas.FindAsync(id);

            if (agenda == null)
            {
                return NotFound();
            }

            return agenda;
        }
    }
}
