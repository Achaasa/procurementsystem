using Microsoft.AspNetCore.Mvc;

namespace procurementsystem.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Health : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
} 