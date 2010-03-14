using Funq;
using StorEvil.Configuration;

namespace StorEvil.Console
{
    public abstract class ContainerConfigurator<settingsT> : IContainerConfigurator where settingsT : new()
    {
        protected readonly SwitchParser<settingsT> SwitchParser;

        public void SetupContainer(Container container, ConfigSettings ConfigSettings, string[] args)
        {
            var settings = new settingsT();
            SwitchParser.Parse(args, settings);
            container.Register(settings);
            SetupCustomComponents(container);
        }

        protected abstract void SetupCustomComponents(Container container);

        protected ContainerConfigurator()
        {
            SwitchParser = new SwitchParser<settingsT>();
            SetupSwitches(SwitchParser);
        }

        public string GetUsage()
        {
            return SwitchParser.GetUsage();
        }

        protected abstract void SetupSwitches(SwitchParser<settingsT> parser);
    }
}