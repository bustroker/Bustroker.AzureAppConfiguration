using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace Bustroker.AzureAppConfiguration
{
    public class OnDemandAzureAppConfigurationRefresher : IAzureAppConfigurationRefresher
    {
        private readonly List<IConfigurationRefresher> _configurationRefreshers = new List<IConfigurationRefresher>();

        public OnDemandAzureAppConfigurationRefresher(IConfiguration configuration)
        {
            var configurationRoot = configuration as IConfigurationRoot;

            if (configurationRoot == null)
            {
                throw new InvalidOperationException("The 'IConfiguration' object injected in OnDemantConfigurationRefresher is not an 'IConfigurationRoot', and needs to be.");
            }

            foreach (var provider in configurationRoot.Providers)
            {
                if (provider is IConfigurationRefresher refresher)
                {
                    _configurationRefreshers.Add(refresher);
                }
            }
            
        }

        public int RegisteredConfigurationRefreshersCount => _configurationRefreshers.Count;

        public async Task RefreshAllRegisteredKeysAsync()
        {
            var refreshTasks = new List<Task>();
            foreach (var refresher in _configurationRefreshers)
            {
                refreshTasks.Add(refresher.RefreshAsync());
            }
            await Task.WhenAll(refreshTasks);
        }
    }
}
