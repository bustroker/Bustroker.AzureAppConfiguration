using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bustroker.AzureAppConfiguration
{
    public interface IAzureAppConfigurationRefresher
    {
        int RegisteredConfigurationRefreshersCount {get; }

        /// <summary>
        /// Calls TryRefreshAsync in all registered <see cref="IConfigurationRefresher"/>.
        /// All registered keys with expired caché will be refreshed.
        /// Return values are the return values of each IConfigurationRefresher's refresh operation
        /// i.e. whether the refresh operation succeeded
        /// </summary>
        Task RefreshAllRegisteredKeysAsync();
    }
}
