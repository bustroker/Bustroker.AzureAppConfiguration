## Microsoft documentation for dynamic config usage
https://docs.microsoft.com/en-us/azure/azure-app-configuration/enable-dynamic-configuration-aspnet-core?tabs=core3x

## Use OnDemandAzureAppConfigurationRefresher
>**_Note: See Bustroker.AzureAppConfiguration.WebApi usage_**

- Create Azure AppConfiguration service with parámeters:
bustroker-pocs-configuration:AppSettings:Sentinel => 0
bustroker-pocs-configuration:AppSettings:BadGuyName => Mr Orange

- Add a refreshing ConfigurationProvider
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
- Register a configuration section so it's bound against TOptions, and IAzureAppConfigurationRefresher service
```
// Startup.ConfigureServices(...)
services.Configure<AppSettings>(Configuration.GetSection("bustroker-pocs-configuration:AppSettings"));
services.AddScoped<IAzureAppConfigurationRefresher, OnDemandAzureAppConfigurationRefresher>();
```

- Note: DO NOT configure the configuration refreshing middleware (app.UseAzureAppConfiguration()), which is actually the whole point.

- To use the configuration values, inject IOptionsSnapshot into the Controller
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

- To refresh configuration with new values in AzureAppConfig service, inject IAzureAppConfigurationRefresher into a Controller
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

### Remarks
As per documentation, refering to 
```
// Startup.cs

app.UseAzureAppConfiguration();
```

> _The middleware uses the refresh configuration specified in the AddAzureAppConfiguration method in Program.cs to trigger a refresh for each request received by the ASP.NET Core web app. For each request, a refresh operation is triggered and the client library checks if the cached value for the registered configuration setting has expired. If it's expired, it's refreshed._

### But
This is NOT the behaviour I want, because:
- either it makes A LOT of extra calls, if cache expiration is short (and it costs money)
- or there is a LONG time to apply config changes, if fache expiration is long

What I want is:
- Minimum calls to Config API, because I'm short in cash :P
- Short cache expiration because I need the changes are applied ASAP

**So I need to trigger the refresh requests explicitly**

### Implementing my own refreshing cycle
I don't want the refresh triggered by the middleware, but to call it explicitly, so:
- I won't register the AppConfiguration middleware in Startup.Configure
- I'll implement the refresh as it is done in the class Microsoft.Azure.AppConfiguration.AspNetCore.AzureAppConfigurationRefreshMiddleware, found in Microsoft repo https://github.com/Azure/AppConfiguration-DotnetProvider
- **NOTE: it's important to use IOptionsSnapshot, and not IOptions (wouldn't refresh anyway unless restarted the application), nor IOptionsMonitor (could change the values in the middle of a request potentially leading to weird results).**

### pack and push nuget package
```
dotnet pack
dotnet nuget push [package.nupkg] --api-key [APIKEY] --source https://api.nuget.org/v3/index.json
```