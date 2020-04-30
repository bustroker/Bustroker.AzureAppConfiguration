## official Microsoft documentation
https://docs.microsoft.com/en-us/azure/azure-app-configuration/enable-dynamic-configuration-aspnet-core?tabs=core3x

## AzAppSettingsController
- Parámetros en Az AppConfiguration
arch-pocs-configuration:AppSettings:Sentinel => 1
arch-pocs-configuration:AppSettings:TerminalId => mr orange

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
- Minimum calls to Config API 
- Short cache expiration because I need the changes are applied ASAP

**So I need to trigger the refresh requests explicitly**

### Implementing my own refreshing cycle
I don't want the refresh triggered by the middleware, but to call it explicitely, so:
- I won't register the AppConfiguration middleware in Startup.Configure
- I'll implement the refresh as it is done in the class Microsoft.Azure.AppConfiguration.AspNetCore.AzureAppConfigurationRefreshMiddleware, found in Microsoft repo https://github.com/Azure/AppConfiguration-DotnetProvider
- NOTE: it's important to use IOptionsSnapshot, and not IOptions (wouldn't refresh anyway unless restarted the application), nor IOptionsMonitor (could change the values in the middle of a request potentially leading to weird results).

