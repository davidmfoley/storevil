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

    public abstract class ContainerConfigurator<settingsT> {
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
            container.EasyRegister<InterpreterForTypeFactory>();
            container.EasyRegister<ExtensionMethodHandler>();

            container.Register<IStoryContextFactory>(GetStoryContextFactory(settings));
        }

        private StoryContextFactory GetStoryContextFactory(ConfigSettings settings)
        {
            var mapper = new StoryContextFactory();
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