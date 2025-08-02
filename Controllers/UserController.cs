using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostSQLgreAPI.Data;
//using Supabase.Gotrue;
using PostSQLgreAPI.Model;

namespace PostSQLgreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Users user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.username))
            {
                return BadRequest(new { message = "Invalid user data." });
            }

            var existingUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.username == user.username);

            if (existingUser != null)
            {
                return Conflict(new { message = "Username already taken." });
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = user.id,
                username = user.username
            });
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Users login)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.username == login.username && u.password == login.password);
            if (user == null) return Unauthorized("Invalid credentials");
            return Ok(1);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Users.ToListAsync());



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Users updated)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.username = updated.username;
        
            user.password = updated.password;
            await _context.SaveChangesAsync();
            return Ok(user);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

}
