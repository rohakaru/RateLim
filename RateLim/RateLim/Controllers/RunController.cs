using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RateLim.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class RunController : ControllerBase
    {
        // GET: v1/<RunController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
