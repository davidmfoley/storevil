using System;
using System.Linq;
using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Utility;

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
            if (command == null || command == "help")
                return new HelpContainerConfigurator();

            //if (CommandRequiresValidSettings(command))
            //    SanityCheckSettings();

            if (command == "nunit")
                return new NUnitContainerSwitchConfigurator();

            if (command == "execute")
                return new InPlaceContainerSwitchConfigurator();

            if (command == "debug")
                return new InPlaceDebugContainerSwitchConfigurator();

            if (command == "init")
                return new InitContainerSwitchConfigurator();

            if (command == "stub")
                return new StubGeneratorContainerSwitchConfigurator();

            if (command == "glossary")
                return new GlossarySwitchConfigurator();

            return new HelpContainerConfigurator();
        }

        private bool CommandRequiresValidSettings(string command)
        {
            return command == "nunit" || command == "execute" || command == "debug";
        }

        private void SanityCheckSettings()
        {
            if (_settings.AssemblyLocations != null && _settings.AssemblyLocations.Any()) 
                return;

            System.Console.WriteLine(
                "Error!\r\n" + 
                "You need to specify paths to your context assembly locations: \r\n" + 
                "Add the \"Assemblies\" setting to your storevil.config\r\n" + 
                "or use the --assemblies command-line switch.");

            Environment.Exit(-1);
        }
    }

   
}