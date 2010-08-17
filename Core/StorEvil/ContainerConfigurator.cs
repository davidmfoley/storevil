using Funq;
using StorEvil.Configuration;
using StorEvil.Console;
using StorEvil.Context;
using StorEvil.Events;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Interpreter.ParameterConverters;
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


            var bus = StorEvilEvents.Bus;
            container.Register<IEventBus>(bus);

            listenerBuilder.SetUpListeners(bus);

            var assemblyRegistry = new AssemblyRegistry(settings.AssemblyLocations);
            container.Register(assemblyRegistry);

            new EventBusAutoRegistrar(assemblyRegistry).InstallTo(bus);

            container.EasyRegister<IStoryParser, StoryParser>();
            container.EasyRegister<IStoryProvider, StoryProvider>();
            container.EasyRegister<IStoryReader, FilesystemStoryReader>();

            container.EasyRegister<IFilesystem, Filesystem>();
            container.EasyRegister<IScenarioPreprocessor, ScenarioPreprocessor>();
            container.EasyRegister<ScenarioInterpreter>();
            container.EasyRegister<IInterpreterForTypeFactory, InterpreterForTypeFactory>();
            container.EasyRegister<ExtensionMethodHandler>();

            container.EasyRegister<IAmbiguousMatchResolver, MostRecentlyUsedContext>();
            
           // container.Register<StorEvilSession>(GetSession(settings));
            container.Register<ISessionContext>(GetSessionContext(settings));
        }

        private ISessionContext GetSessionContext(ConfigSettings settings)
        {
            var assemblyRegistry = new AssemblyRegistry(settings.AssemblyLocations);
            ParameterConverter.AddCustomConverters(assemblyRegistry);

            var sessionContext = new SessionContext(assemblyRegistry);
            return sessionContext;
        }

        protected abstract void SetupCustomComponents(Container container, ConfigSettings configSettings, settingsT customSettings);
    }
}