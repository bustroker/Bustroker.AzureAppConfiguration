using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bustroker.AzureAppConfiguration.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AzAppConfigurationController : ControllerBase
    {
        private readonly AppSettings _appSettings;

        public AzAppConfigurationController(IOptionsSnapshot<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Task.CompletedTask;
            return Ok(_appSettings);
        }
    }
}
