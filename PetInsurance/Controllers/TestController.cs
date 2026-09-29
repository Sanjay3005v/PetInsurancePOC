using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PetInsurance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Pet Insurane API Running");
        }
    }
}
