using System;
using System.Linq;
using StorEvil.Configuration;

namespace StorEvil.Console
{
    internal class ContainerConfiguratorFactory
    {
        private readonly ConfigSettings _settings;

        public ContainerConfiguratorFactory(ConfigSettings settings)
        {
            _settings = settings;
        }

        public IContainerConfigurator GetConfigurator(string command)
        {
            IContainerConfigurator containerConfigurator = null;

            if (command == null || command == "help")
                return new HelpContainerConfigurator();

            if (command == "nunit" || command == "execute")
                SanityCheckSettings();

            if (command == "nunit")
                containerConfigurator = new NUnitContainerConfigurator();
            else if (command == "execute")
                containerConfigurator = new InPlaceContainerConfigurator();
            else if (command == "init")
                containerConfigurator = new InitContainerConfigurator();

            return containerConfigurator ?? new HelpContainerConfigurator();
        }

        private void SanityCheckSettings()
        {
            if (_settings.AssemblyLocations == null || !_settings.AssemblyLocations.Any())
            {
                System.Console.WriteLine(
                    "Error!\r\nYou need to specify paths to your context assembly locations \r\n(setting: \"Assemblies\") in storevil.config");
                Environment.Exit(1);
            }
        }
    }
}