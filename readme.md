## Use OnDemandAzureAppConfigurationRefresher
>**_Note: See Bustroker.AzureAppConfiguration.WebApi usage_**

### Create Azure AppConfiguration service with parámeters:

bustroker-pocs-configuration:AppSettings:Sentinel => 0
bustroker-pocs-configuration:AppSettings:BadGuyName => Mr Orange

### Add a refreshing ConfigurationProvider
```
// Program.CreateHostBuilder(...)
webBuilder                        
    .ConfigureAppConfiguration((hostingContext, config) =>
        {
            var settings = config.Build();
            config.AddAzureAppConfiguration(options =>
            {
                options.Connect(settings["ConnectionStrings:AzAppConfigurationConnectionString"])
                        .ConfigureRefresh(refreshOptions =>
                        {
                            refreshOptions.Register("bustroker-pocs-configuration:AppSettings:sentinel", refreshAll: true)
                                    .SetCacheExpiration(TimeSpan.FromSeconds(1));
                        });
            });
        })
```
### Register a configuration section so it's bound against TOptions, and IAzureAppConfigurationRefresher service
```
// Startup.ConfigureServices(...)
services.Configure<AppSettings>(Configuration.GetSection("bustroker-pocs-configuration:AppSettings"));
services.AddScoped<IAzureAppConfigurationRefresher, OnDemandAzureAppConfigurationRefresher>();
```

>**Note: DO NOT configure the configuration refreshing middleware (app.UseAzureAppConfiguration()), which is actually the whole point.**

### To use the configuration values, inject IOptionsSnapshot into the Controller
```
// AzAppConfigurationController.cs
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
```

- **NOTE: it's important to use IOptionsSnapshot, and not IOptions (wouldn't refresh anyway unless restarted the application), nor IOptionsMonitor (could change the values in the middle of a request potentially leading to weird results).**

### To refresh configuration with new values in AzureAppConfig service, inject IAzureAppConfigurationRefresher into a Controller
```
// RefreshAzAppConfigurationController

[ApiController]
[Route("[controller]")]
public class RefreshAzAppConfigurationController : ControllerBase
{
    private readonly IAzureAppConfigurationRefresher _azureAppConfigurationRefresher;

    public RefreshAzAppConfigurationController(IAzureAppConfigurationRefresher onDemandConfigurationRefresher)
    {
        _azureAppConfigurationRefresher = onDemandConfigurationRefresher;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _azureAppConfigurationRefresher.TryRefreshAllRegisteredKeysAsync();
        return Ok(result);
    }
}
```
