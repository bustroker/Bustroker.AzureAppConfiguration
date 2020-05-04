﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public OnDemandAzureAppConfigurationRefresher(IEnumerable<IConfigurationRefresher> configurationRefreshers)
        {
            _configurationRefreshers = configurationRefreshers.ToList();
        }

        public int RegisteredConfigurationRefreshersCount => _configurationRefreshers.Count;

        public async Task RefreshAllRegisteredKeysAsync()
        {
            Task compositeTask = null;
            var refreshersTasks = new List<Task>();
            try
            {
                _configurationRefreshers.ForEach(r => refreshersTasks.Add(r.RefreshAsync()));
                compositeTask = Task.WhenAll(refreshersTasks);
                await compositeTask;
            }
            catch (Exception)
            {
                throw compositeTask.Exception;
            }
        }
    }
}
