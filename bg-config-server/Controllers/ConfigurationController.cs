using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace bg_config_server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET values
        [HttpGet]
        public ActionResult<IEnumerable<KeyValuePair<string, string>>> Get()
        {
            return Ok(_configuration.AsEnumerable());
        }
    }
}
