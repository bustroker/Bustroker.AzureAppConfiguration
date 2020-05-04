using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bustroker.AzureAppConfiguration.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RefreshAzAppConfigurationController : ControllerBase
    {
        private readonly IAzureAppConfigurationRefresher _azureAppConfigurationRefresher;

        public RefreshAzAppConfigurationController(IAzureAppConfigurationRefresher onDemandConfigurationRefresher)
        {
            _azureAppConfigurationRefresher = onDemandConfigurationRefresher;
        }

        [HttpGet, Route("count")]
        public async Task<IActionResult> RegisteredRefreshersCount()
        {
            await Task.CompletedTask;
            return Ok(_azureAppConfigurationRefresher.RegisteredConfigurationRefreshersCount);
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _azureAppConfigurationRefresher.RefreshAllRegisteredKeysAsync();
            return Ok();
        }
    }
}
