using System.IO;
using Funq;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Parsing;
using StorEvil.Utility;

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
            SetupCommonComponents(Container);
            SetupCustomComponents(Container, args);

            return Container.Resolve<IStorEvilJob>();
        }

        private ConfigSettings ParseCommonConfigSettings(Container container, string[] args)
        {
            SwitchParser<ConfigSettings> switchParser = new CommonSwitchParser();

            var settings = _configSource.GetConfig(Directory.GetCurrentDirectory());
            settings.StoryBasePath = Directory.GetCurrentDirectory();
            switchParser.Parse(args, settings);

            container.Register(settings);

            return settings;
        }

        private void SetupCommonComponents(Container container)
        {
            var listenerBuilder = new ListenerBuilder(_settings);
            container.Register(listenerBuilder.GetResultListener());

            container.EasyRegister<IStoryParser, StoryParser>();
            container.EasyRegister<IStoryProvider, StoryProvider>();
            container.EasyRegister<IStoryReader, FilesystemStoryReader>();

            container.EasyRegister<IFilesystem, Filesystem>();
            container.EasyRegister<IScenarioPreprocessor, ScenarioPreprocessor>();
            container.EasyRegister<ScenarioInterpreter>();
            container.EasyRegister<InterpreterForTypeFactory>();
            container.EasyRegister<ExtensionMethodHandler>();

            container.Register<IStoryToContextMapper>(GetStoryToContextMapper());
        }

        private StoryToContextMapper GetStoryToContextMapper()
        {
            var mapper = new StoryToContextMapper();
            foreach (var location in _settings.AssemblyLocations)
                mapper.AddAssembly(location);
            return mapper;
        }

        private void SetupCustomComponents(Container container, string[] args)
        {
            var command = args.Length > 0 ? args[0] : "help";

            var configurator = new ContainerConfiguratorFactory(_settings).GetConfigurator(command);

            configurator.SetupContainer(container, _settings, args);
        }
    }
}