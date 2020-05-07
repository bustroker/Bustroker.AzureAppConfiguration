using System.Collections.Generic;
using System.Linq;

namespace Bustroker.AzureAppConfiguration.WebApi
{
    public class AppSettings
    {
        public string CsvValues { get; set; }
        public int IntegerValue { get; set; }
        public string SecretFromKeyVault { get; set; }

        // This is not mapped, it's just a pretty format for CsvValues kept in config param
        public List<string> ListOfCsvValues => CsvValues.Split(",").ToList();
    }
}