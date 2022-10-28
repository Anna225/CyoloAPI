using LawyerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Extensions;

namespace LawyerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomController : ControllerBase
    {
        private readonly LawyerDbContext _context;

        public CustomController(LawyerDbContext context)
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

        // GET: api/Custom/CourtCaseByDateAndCourtName/2022-10-03/democourtname
        [HttpGet("CourtCaseByDateAndCourtName/{date}/{courtname}")]
        public async Task<ActionResult<List<CourtCaseAgenda?>>> GetCourtCaseByDateAndCourtName(string date, string courtname)
        {
            return await _context.CourtCaseAgenda.AsNoTracking()
                .Where(x => x.HearingDate == date && (x.HearingGeneral!.Contains(courtname)))
                .GroupBy(x => x.HearingGeneral)
                .Select(m => m.FirstOrDefault())
                .ToListAsync();            
        }

        // GET: api/Custom/CourtCaseBydateAndEmail/2022-10-03/demoemail
        [HttpGet("CourtCaseByDateAndEmail/{date}/{lawyeremail}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetCourtCaseByDateAndEmail(string date, string lawyeremail)
        {
            var lawyers = (from courtcase in _context.CourtCaseAgenda
                           join lawyer in _context.Lawyers
                           on new { LawyerName = courtcase.LawyerName, LawyerSurename = courtcase.LawyerSurename }
                           equals new { LawyerName = lawyer.Name, LawyerSurename = lawyer.SureName }
                           where lawyer.Email!.Contains(lawyeremail) &&
                                   (courtcase.HearingDate == date)
                           select new { courtcase, lawyer }).Distinct();

            if (lawyers == null)
            {
                return NotFound();
            }
            return await lawyers.ToListAsync();
        }

        // GET: api/Custom/LawyersByCourtCaseId/234
        [HttpGet("LawyersByCourtCaseId/{courtcaseid}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetLawyersByCourtCaseId(string courtcaseid)
        {
            var queryParam = System.Uri.UnescapeDataString(courtcaseid);
            var lawyers = (from lawyer in _context.Lawyers
                           join courtcase in _context.CourtCaseAgenda
                           on new { Name = lawyer.Name, Surename = lawyer.SureName }
                           equals new { Name = courtcase.LawyerName, Surename = courtcase.LawyerSurename }
                           where (courtcase.CourtCaseNo!.Replace(" ", "")!.Contains(queryParam.Replace(" ", "")))
                           select new { lawyer, courtcase }).Distinct();

            if (lawyers == null)
            {
                return NotFound();
            }
            return await lawyers.ToListAsync();
        }
        
        // GET: api/Custom/CourtCaseByDateAndCourtName/2022-10-03/democourtname
        [HttpGet("AllCourtCasesByDate/{date}")]
        public async Task<ActionResult<List<CourtCaseAgenda?>>> GetAllCourtCasesByDate(string date)
        {
            return await _context.CourtCaseAgenda.AsNoTracking()
                .Where(x => x.HearingDate == date)
                .GroupBy(x => x.CourtCaseNo)
                .Select(m => m.FirstOrDefault())
                .ToListAsync();            
        }

        // GET: api/Custom/CourtCaseByDateAndEmail/2022-10-03/name
        [HttpGet("CourtCaseByDateAndName/{date}/{lawyername}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetCourtCaseByDateAndName(string date, string lawyername)
        {
            var lawyers = (from courtcase in _context.CourtCaseAgenda
                           join lawyer in _context.Lawyers
                           on new { LawyerName = courtcase.LawyerName, LawyerSurename = courtcase.LawyerSurename }
                           equals new { LawyerName = lawyer.Name, LawyerSurename = lawyer.SureName }
                           where (lawyer.Name + " " + lawyer.SureName == lawyername) &&
                                   (courtcase.HearingDate == date)
                           select new { courtcase, lawyer }).Distinct();

            if (lawyers == null)
            {
                return NotFound();
            }
            return await lawyers.ToListAsync();
        }

        // GET: api/Custom/CourtCaseBydateAndPhone/2022-10-03/phonenumber
        [HttpGet("CourtCaseByDateAndPhone/{date}/{phone}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetCourtCaseByDateAndPhone(string date, string phone)
        {
            var lawyers = (from courtcase in _context.CourtCaseAgenda
                           join lawyer in _context.Lawyers
                           on new { LawyerName = courtcase.LawyerName, LawyerSurename = courtcase.LawyerSurename }
                           equals new { LawyerName = lawyer.Name, LawyerSurename = lawyer.SureName }
                           where lawyer.Phone!.Replace(" ", "").Trim().Contains(phone.Replace(" ", "").Trim()) &&
                                   (courtcase.HearingDate == date)
                           select new { courtcase, lawyer }).Distinct();

            if (lawyers == null)
            {
                return NotFound();
            }
            return await lawyers.ToListAsync();
        }



        // GET: api/GetLawyerByEmail/admin@gmail.com
        [HttpGet("GetLawyerByEmail/{email}")]
        public async Task<ActionResult<Lawyer>> GetLawyerByEmail(string email)
        {
            if (_context.Lawyers == null)
            {
                return NotFound();
            }
            var lawyer = await _context.Lawyers.Where(z => z.Email!.Contains(email)).FirstOrDefaultAsync();

            if (lawyer == null)
            {
                return NotFound();
            }

            return lawyer;
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

        // GET: api/Custom/AllCourtTypes
        [HttpGet("AllCourtTypes")]
        public async Task<ActionResult<List<string?>>> GetAllCourtTypes()
        {
            return await _context.CourtCaseAgenda.AsNoTracking()
                .Where(y => !string.IsNullOrEmpty(y.CourtType))
                .OrderBy(o => o.CourtType)
                .Select(m => m.CourtType)
                .Distinct()
                .ToListAsync();
        }

        // GET: api/Custom/AllChamberIDs
        [HttpGet("AllChamberIDs")]
        public async Task<ActionResult<List<string?>>> GetAllChamberIDs()
        {
            return await _context.CourtCaseAgenda.AsNoTracking()
                .Where(y => !string.IsNullOrEmpty(y.ChamberID))
                .OrderBy(o => o.ChamberID)
                .Select(m => m.ChamberID)
                .Distinct()
                .ToListAsync();
        }

        // GET: api/Custom/AllCourtLocations
        [HttpGet("AllCourtLocations")]
        public async Task<ActionResult<List<string?>>> GetAllCourtLocations()
        {
            return await _context.CourtCaseAgenda.AsNoTracking()
                .Where(y => !string.IsNullOrEmpty(y.CourtLocation))
                .OrderBy(o => o.CourtLocation)
                .Select(m => m.CourtLocation)
                .Distinct()
                .ToListAsync();
        }

    }
}
