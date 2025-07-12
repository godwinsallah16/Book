using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                status = "Healthy", 
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                service = "BookStore.API"
            });
        }
    }
}
