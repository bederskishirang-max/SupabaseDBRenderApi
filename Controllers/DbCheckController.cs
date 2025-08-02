using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace PostSQLgreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DbCheckController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DbCheckController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand("SELECT NOW()", conn);
                var result = await cmd.ExecuteScalarAsync();

                return Ok(new
                {
                    message = "Connected to portfolio-db successfully!",
                    server_time = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Connection failed",
                    error = ex.Message
                });
            }
        }


    }
}
