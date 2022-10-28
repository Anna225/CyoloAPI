using LawyerAPI.Helper;
using LawyerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawyerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LawyerDbContext _context;
        public AuthController(LawyerDbContext context)
        {
            _context = context;
        }

        // POST: Register
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Register")]
        public async Task<ActionResult<bool>> Register(UserDto userdto)
        {

            if (_context.Users == null)
            {
                return Problem("Entity set 'LawyerDbContext.Users'  is null.");
            }

            _context.Users.Add(new User
            {
                Email = userdto.Email,
                Password = userdto.Password,
                Remember_token = userdto.Remember_token,
            });
            int status = await _context.SaveChangesAsync();
            if (status != 1)
            {
                return false;
            }
            return true;
        }

        // POST: Login
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Login")]
        public ActionResult<bool> Login(UserDto userdto)
        {

            var user = _context.Users
                .Where(x => x.Email == userdto.Email)
                .Where(x => x.Password == userdto.Password)
                .FirstOrDefault<User>();

            if (user == null)
            {
                return false;
            }

            return true;
        }

        // Post: IsExist
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("IsExist")]
        public ActionResult<bool> IsExist(UserDto userdto)
        {

            var user = _context.Users
                .Where(x => x.Email == userdto.Email)
                .FirstOrDefault<User>();

            if (user == null)
            {
                return false;
            }

            return true;
        }

        // GET: api/Auth/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {

            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

    }
}
