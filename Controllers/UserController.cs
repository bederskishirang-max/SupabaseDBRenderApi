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
            try
            {
                // Check for existing email and username using AsNoTracking for performance
                var emailExists = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.email == user.email);

                if (emailExists)
                    return Conflict(new { message = "Email already registered." });

                var usernameExists = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.username == user.username);

                if (usernameExists)
                    return Conflict(new { message = "Username already taken." });

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = user.id,
                    email = user.email,
                    username = user.username
                });
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message?.Contains("duplicate key value") == true)
            {
                return Conflict(new
                {
                    message = "A duplicate key error occurred. Either the email or username already exists.",
                    details = dbEx.InnerException.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
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
            user.email = updated.email;
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
