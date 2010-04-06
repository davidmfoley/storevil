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
    public abstract class ContainerConfigurator<settingsT> : IContainerConfigurator where settingsT : new()
    {
        protected readonly SwitchParser<settingsT> SwitchParser;
        protected settingsT CustomSettings;

        protected ContainerConfigurator()
        {
            SwitchParser = new SwitchParser<settingsT>();
            SetupSwitches(SwitchParser);
        }

        public void SetupContainer(Container container, ConfigSettings configSettings, string[] args)
        {
            CustomSettings = new settingsT();
            SwitchParser.Parse(args, CustomSettings);
            container.Register(CustomSettings);
            SetupCommonComponents(container, configSettings);
            SetupCustomComponents(container);
        }

        private void SetupCommonComponents(Container container, ConfigSettings settings)
        {
            var listenerBuilder = new ListenerBuilder(settings);
            if (settings.Debug)
            {
                DebugTrace.Listener = new ConsoleDebugListener();
            }   
        
            container.Register(listenerBuilder.GetResultListener());

            container.EasyRegister<IStoryParser, StoryParser>();
            container.EasyRegister<IStoryProvider, StoryProvider>();
            container.EasyRegister<IStoryReader, FilesystemStoryReader>();

            container.EasyRegister<IFilesystem, Filesystem>();
            container.EasyRegister<IScenarioPreprocessor, ScenarioPreprocessor>();
            container.EasyRegister<ScenarioInterpreter>();
            container.EasyRegister<InterpreterForTypeFactory>();
            container.EasyRegister<ExtensionMethodHandler>();

            container.Register<IStoryContextFactory>(GetStoryToContextMapper(settings));
        }

        private StoryContextFactory GetStoryToContextMapper(ConfigSettings settings)
        {
            var mapper = new StoryContextFactory();
            foreach (var location in settings.AssemblyLocations)
            {
                DebugTrace.Trace(this.GetType().Name, "Adding context assembly:" + location);
                mapper.AddAssembly(location);
            }
            return mapper;
        }

        protected abstract void SetupCustomComponents(Container container);

       

        public string GetUsage()
        {
            return SwitchParser.GetUsage();
        }

        protected abstract void SetupSwitches(SwitchParser<settingsT> parser);
    }
}