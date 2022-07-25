using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityEntityFrameworkCore.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet, Route("GetProducts")]
        public IActionResult GetProducts()
        {
            return Ok(new List<string> { "pro1", "pro2", "pro3" });
        }
    }
}
