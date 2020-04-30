
## Scenario, intent & remarks
As per documentation (https://docs.microsoft.com/en-us/azure/azure-app-configuration/enable-dynamic-configuration-aspnet-core?tabs=core3x), refering to 
```
// Startup.cs
app.UseAzureAppConfiguration();
```

> _The middleware uses the refresh configuration specified in the AddAzureAppConfiguration method in Program.cs to trigger a refresh for each request received by the ASP.NET Core web app. For each request, a refresh operation is triggered and the client library checks if the cached value for the registered configuration setting has expired. If it's expired, it's refreshed._

### But
It's worth to note that the refresh request is sent and not waited for, so it actually does not impact in the incomming request. Rather the next request is the one that will see the change in the value. BUT, it impacts in cost, because considering a cache of 30 sec (the default) and a permanently in use API, you get one request to the 'sentinel' key every 30 seconds to the App Config Service, PER SERVICE with this configurations. So, 10 services make over 200k calls a day, and that is actually the limit above which the service starts charging extra. Not good.

So, this is NOT the behaviour I want, because:
- either it makes A LOT of extra calls, if cache expiration is short (and it costs money)
- or there is a LONG time to apply config changes, if fache expiration is long

What I want is:
- Minimum calls to Config API, because I'm short in cash :P
- Short cache expiration because I need the changes are applied ASAP

## Implementing my own refreshing cycle
I don't want the refresh triggered by the middleware, but to call it explicitly, so:
- I won't register the AppConfiguration middleware in Startup.Configure
- I'll implement the refresh as it is done in the class Microsoft.Azure.AppConfiguration.AspNetCore.AzureAppConfigurationRefreshMiddleware, found in Microsoft repo https://github.com/Azure/AppConfiguration-DotnetProvider


### pack and push nuget package
```
dotnet pack
dotnet nuget push [package.nupkg] --api-key [APIKEY] --source https://api.nuget.org/v3/index.json
```