using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;

namespace E_Commerce.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    [Authorize]
    public class DemoController : ControllerBase
    {
        
        [HttpGet("HelloWorld")]
        public string PrintHelloWorld()
        {
            return "HelloWorld";
        }
    }
}
