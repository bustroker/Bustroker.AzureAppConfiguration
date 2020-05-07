## Use OnDemandAzureAppConfigurationRefresher
>**_Note: See Bustroker.AzureAppConfiguration.WebApi usage_**

### Create Azure AppConfiguration service with parámeters:
All params will observe the format _Bustroker.AzureAppConfiguration.WebApi:[ParamName]_, and _Bustroker.AzureAppConfiguration.WebApi:_ will be as a section in the AppSettings.json. 
It's the section to be mapped to AppSettings class created later on.

The params in our case would be (let's create various types/options):
Bustroker.AzureAppConfiguration.WebApi:Sentinel => 0
Bustroker.AzureAppConfiguration.WebApi:CsvValues => Mr Orange,Mr Pink,Mr Brown
Bustroker.AzureAppConfiguration.WebApi:IntegerValue => 69
Bustroker.AzureAppConfiguration.WebApi:SecretInKeyvault => The value is kept in key vault. When creating, just select "Key Vault Reference", rather than "Key-value".

### Endpoints
/azappconfiguration => will return all the params in AppSettings
/refreshazappconfiguration => will refresh all params, i.e., if _sentinel_ has changed, all the config will be refreshed.
/refreshazappconfiguration/count => just to check how many IConfigurationRefreshers have been configured.