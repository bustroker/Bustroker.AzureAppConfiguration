### pack and push nuget package
set nuget package version in Bustroker.AzureAppConfiguration.csproj
```
dotnet pack
dotnet nuget push [package.nupkg] --api-key [APIKEY] --source https://api.nuget.org/v3/index.json
```