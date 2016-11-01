using Microsoft.Extensions.Configuration;
using System.IO;

namespace TestChessterUciCore
{
    public class TestUtility
    {

        public TestUtility()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(currentDirectory);
            configurationBuilder.AddJsonFile("config.json");
            Config = configurationBuilder.Build();
        }

        public IConfigurationRoot Config { get; private set; }
    }
}
