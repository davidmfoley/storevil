using System;
using Funq;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.Console
{
    public abstract class ContainerSwitchConfigurator<settingsT> : IContainerConfigurator where settingsT : new()
    {
        protected readonly SwitchParser<settingsT> SwitchParser;
        protected settingsT CustomSettings;
        protected ContainerConfigurator<settingsT> _configurator;
        protected ContainerSwitchConfigurator(ContainerConfigurator<settingsT> configurator)
        {
            _configurator = configurator;
            SwitchParser = new SwitchParser<settingsT>();
            SetupSwitches(SwitchParser);
        }
        protected abstract void SetupSwitches(SwitchParser<settingsT> parser);

        public string GetUsage()
        {
            return SwitchParser.GetUsage();
        }

        public void SetupContainer(Container container, ConfigSettings configSettings, string[] args)
        {
            CustomSettings = new settingsT();
            SwitchParser.Parse(args, CustomSettings);

            _configurator.ConfigureContainer(container, configSettings, CustomSettings);
        }
    }

    
}