using Funq;
using StorEvil.Core.Configuration;

namespace StorEvil.Console
{
    public abstract class JobFactory<settingsT> : IJobFactory where settingsT : new()
    {
        protected readonly SwitchParser<settingsT> SwitchParser;

        public void SetupContainer(Container container, string[] args)
        {
            var settings = new settingsT();

            SwitchParser.Parse(args, settings);

            container.Register(settings);

            SetupCustomComponents(container);
        }

        protected abstract void SetupCustomComponents(Container container);

        protected JobFactory()
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