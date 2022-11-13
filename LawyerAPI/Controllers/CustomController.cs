using LawyerAPI.Helper;
using LawyerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                           orderby courtcase.HearingDate descending, courtcase.HearingTime descending
                           select new { courtcase, lawyer }).Distinct();

            if (lawyers == null)
            {
                return NotFound();
            }
            return await lawyers.ToListAsync();
        }

        // GET: api/Custom/CourtCaseByEmail/demoemail
        [HttpGet("CourtCaseByEmail/{lawyeremail}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetCourtCaseByEmail(string lawyeremail)
        {
            var lawyers = (from courtcase in _context.CourtCaseAgenda
                           join lawyer in _context.Lawyers
                           on new { LawyerName = courtcase.LawyerName, LawyerSurename = courtcase.LawyerSurename }
                           equals new { LawyerName = lawyer.Name, LawyerSurename = lawyer.SureName }
                           where lawyer.Email!.Contains(lawyeremail)
                           orderby courtcase.HearingDate descending, courtcase.HearingTime descending
                           select new { courtcase, lawyer }).Distinct();

            if (lawyers == null)
            {
                return NotFound();
            }
            return await lawyers.ToListAsync();
        }

        // GET: api/Custom/GetAgendasByEmail/demoemail
        [HttpGet("GetAgendasByEmail/{lawyeremail}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAgendasByEmail(string lawyeremail)
        {
            var lawyers = (from agenda in _context.Agendas
                           where agenda.UploaderEmail!.Contains(lawyeremail)
                           orderby agenda.HearingDate descending, agenda.HearingTime descending
                           select new { agenda }).Distinct();

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
                           where (courtcase.CourtCaseNo!.Replace(" ", "").Contains(queryParam.Replace(" ", "")))
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
                           orderby courtcase.HearingDate descending, courtcase.HearingTime descending
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
                           orderby courtcase.HearingDate descending, courtcase.HearingTime descending
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

        // GET: api/Custom/AllJurisdictions
        [HttpGet("AllJurisdictions")]
        public async Task<ActionResult<List<string?>>> GetAllJurisdictions()
        {
            return await _context.CourtCaseAgenda.AsNoTracking()
                .Where(y => !string.IsNullOrEmpty(y.HearingGeneral))
                .OrderBy(o => o.HearingGeneral)
                .Select(m => m.HearingGeneral)
                .Distinct()
                .ToListAsync();
        }

        // GET: api/Custom/Test
        [HttpGet("Test")]
        public async Task<Tuple<int, int, int, IEnumerable<CourtCaseResponseDto>>> GetCourtCaseByDateAndEmailTest(
            int draw,
            string date,
            string lawyername,
            string sortColumn,
            string sortColumnDirection = "asc",
            string searchValue = "",
            int pageSize = 10,
            int skip = 0
        )
        {
            var records = _context.CourtCaseAgenda.AsQueryable();
            //get total count of data in table
            int totalRecord = records.Count();
            // search data when search value found
            if (!string.IsNullOrEmpty(searchValue))
            {
                records = records.Where(
                    x => x.CourtCaseNo!.Contains(searchValue)
                    || x.HearingGeneral!.Contains(searchValue)
                    || x.ChamberID!.Contains(searchValue)
                    || x.HearingTime!.Contains(searchValue)
                    || x.HearingDate!.Contains(searchValue));
            }
            records = records.Where(
                    x => (x.LawyerName!.ToLower() + " " + x.LawyerSurename!.ToLower() == lawyername));
            records = records.Where(
                    x => (x.HearingDate == date));
            // get total count of records after search
            int filterRecord = records.Count();
            //sort data
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                //CourtCaseNo
                if (sortColumn == "CourtCaseNo" && sortColumnDirection == "asc")
                {
                    records = records.OrderBy(x => x.CourtCaseNo);
                }
                if (sortColumn == "CourtCaseNo" && sortColumnDirection == "desc")
                {
                    records = records.OrderByDescending(x => x.CourtCaseNo);
                }
                //HearingGeneral
                if (sortColumn == "HearingGeneral" && sortColumnDirection == "asc")
                {
                    records = records.OrderBy(x => x.HearingGeneral);
                }
                if (sortColumn == "HearingGeneral" && sortColumnDirection == "desc")
                {
                    records = records.OrderByDescending(x => x.HearingGeneral);
                }
                //HearingTime
                if (sortColumn == "HearingTime" && sortColumnDirection == "asc")
                {
                    records = records.OrderBy(x => x.HearingTime);
                }
                if (sortColumn == "HearingTime" && sortColumnDirection == "desc")
                {
                    records = records.OrderByDescending(x => x.HearingTime);
                }
                //HearingDate
                if (sortColumn == "HearingDate" && sortColumnDirection == "asc")
                {
                    records = records.OrderBy(x => x.HearingDate);
                }
                if (sortColumn == "HearingDate" && sortColumnDirection == "desc")
                {
                    records = records.OrderByDescending(x => x.HearingDate);
                }
                //ChamberID
                if (sortColumn == "ChamberID" && sortColumnDirection == "asc")
                {
                    records = records.OrderBy(x => x.ChamberID);
                }
                if (sortColumn == "ChamberID" && sortColumnDirection == "desc")
                {
                    records = records.OrderByDescending(x => x.ChamberID);
                }
            }
                
            //pagination
            var courtList = records.Skip(skip).Take(pageSize).Select(x => new CourtCaseResponseDto
            {
                CourtCaseNo = x.CourtCaseNo,
                HearingGeneral = x.HearingGeneral,
                HearingDate = x.HearingDate,
                HearingTime = x.HearingTime,
                ChamberID = x.ChamberID,
                HearingType = x.HearingType
            }).ToList();

            Tuple<int, int, int, IEnumerable<CourtCaseResponseDto>> returnObj = new Tuple<int, int, int, IEnumerable<CourtCaseResponseDto>>
            (
                draw,
                totalRecord,
                filterRecord,
                courtList
            );           

            return await Task.FromResult(returnObj);
        }

    }
}
