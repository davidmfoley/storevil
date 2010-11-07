using System.IO;
using Funq;
using StorEvil.Configuration;
using StorEvil.Core;

namespace StorEvil.Console
{
    public class JobFactory
    {
        private readonly IConfigSource _configSource;
        private ConfigSettings _settings;

        public Container Container { get; private set; }

        public JobFactory(IConfigSource source)
        {
            _configSource = source;

            Container = new Container();
        }

        public IStorEvilJob ParseArguments(string[] args)
        {
            _settings = ParseCommonConfigSettings(Container, args);

            SetupContainer(Container, args);

            return Container.Resolve<IStorEvilJob>();
        }

        private ConfigSettings ParseCommonConfigSettings(Container container, string[] args)
        {
            SwitchParser<ConfigSettings> switchParser = new CommonSwitchParser();

            var settings = _configSource.GetConfig(Directory.GetCurrentDirectory());
            if (string.IsNullOrEmpty(settings.StoryBasePath))
                settings.StoryBasePath = Directory.GetCurrentDirectory();

            switchParser.Parse(args, settings);

            container.Register(settings);

            return settings;
        }

        private void SetupContainer(Container container, string[] args)
        {
            var command = args.Length > 0 ? args[0] : "help";

            var configurator = new ContainerConfiguratorFactory(_settings).GetConfigurator(command);

            configurator.SetupContainer(container, _settings, args);
        }
    }
}