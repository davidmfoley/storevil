using Funq;
using StorEvil.Configuration;
using StorEvil.Console;
using StorEvil.Context;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil
{
    public abstract class ContainerConfigurator<settingsT>
    {
        public void ConfigureContainer(Container container, ConfigSettings configSettings, settingsT customSettings)
        {
            container.Register(customSettings);
            SetupCommonComponents(container, configSettings);
            SetupCustomComponents(container, configSettings, customSettings);
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
            container.EasyRegister<IInterpreterForTypeFactory, InterpreterForTypeFactory>();
            container.EasyRegister<ExtensionMethodHandler>();

            container.EasyRegister<IAmbiguousMatchResolver, DisallowAmbiguousMatches>();

            container.Register<ISessionContext>(GetStoryContextFactory(settings));
        }

        private SessionContext GetStoryContextFactory(ConfigSettings settings)
        {
            var mapper = new SessionContext();
            foreach (var location in settings.AssemblyLocations)
            {
                DebugTrace.Trace(this.GetType().Name, "Adding context assembly:" + location);
                mapper.AddAssembly(location);
            }
            return mapper;
        }

        protected abstract void SetupCustomComponents(Container container, ConfigSettings configSettings, settingsT customSettings);
    }
}