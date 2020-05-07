using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bustroker.AzureAppConfiguration.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureAppConfiguration((hostingContext, config) =>
                            {
                                var azAppConfigurationMainRegionUrl = config.Build().GetValue<string>("AzAppConfigurationMainRegionUrl");
                                var azAppConfigurationFailOverRegionUrl = config.Build().GetValue<string>("AzAppConfigurationMainRegionUrl");

                                config
                                    .AddAzureAppConfiguration(options => // optionally configure failover App Configuration for HA
                                    {
                                        options.Connect(new Uri(azAppConfigurationFailOverRegionUrl), new DefaultAzureCredential())
                                            .ConfigureKeyVault(kv => // Configure keyvault so values in AppConfiguration that are in Key Vault can be accessed by the application
                                            {
                                                kv.SetCredential(new DefaultAzureCredential());
                                            })
                                            .ConfigureRefresh(refreshOptions =>
                                            {
                                                refreshOptions.Register("Bustroker.AzureAppConfiguration.WebApi:sentinel", refreshAll: true)
                                                        .SetCacheExpiration(TimeSpan.FromSeconds(1));
                                            });
                                    }, optional: true)
                                    .AddAzureAppConfiguration(options => // configure main App Configuration 
                                    {
                                        options.Connect(new Uri(azAppConfigurationMainRegionUrl), new DefaultAzureCredential())
                                            .ConfigureKeyVault(kv => // Configure keyvault so values in AppConfiguration that are in Key Vault can be accessed by the application
                                            {
                                                kv.SetCredential(new DefaultAzureCredential());
                                            })
                                            .ConfigureRefresh(refreshOptions =>
                                            {
                                                refreshOptions.Register("Bustroker.AzureAppConfiguration.WebApi:sentinel", refreshAll: true)
                                                        .SetCacheExpiration(TimeSpan.FromSeconds(1));
                                            });
                                    }, optional: true);
                            })
                        .UseStartup<Startup>();
                });
    }
}
