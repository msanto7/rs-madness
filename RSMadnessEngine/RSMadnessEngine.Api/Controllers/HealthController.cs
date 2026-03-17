using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Data;
using Microsoft.EntityFrameworkCore;

namespace RSMadnessEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public HealthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Quick controller to test db connectivity from FE to DB and back.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Check database connectivity
                var canConnect = await _dbContext.Database.CanConnectAsync();
                var userCount = canConnect ? await _dbContext.Users.CountAsync() : 0;
                return Ok(new
                {
                    status = "Healthy",
                    database = canConnect ? "Connected" : "Disconnected",
                    userCount,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status503ServiceUnavailable, new { status = "Unhealthy", error = ex.Message });
            }
        }
    }
}
