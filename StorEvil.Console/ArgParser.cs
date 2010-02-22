using System;
using System.IO;
using System.Linq;
using Funq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Nunit;

namespace StorEvil.Console
{
    public class ArgParser
    {
        private readonly IConfigSource _configSource;
        private readonly ConfigSettings _settings;
        private readonly Container _container;

        public ArgParser(IConfigSource source)
        {
            _configSource = source;
            _settings = source.GetConfig(Directory.GetCurrentDirectory());
            _container = new Container();
        }

        public IStorEvilJob ParseArguments(string[] args)
        {
            SetupCommonComponents(_container);

            SetupCustomComponents(_container, args);

            return _container.Resolve<IStorEvilJob>();
        }

        private void SetupCommonComponents(Container container)
        {
            container.Register<ConfigSettings>((c)=>_settings);

            container.EasyRegister<IStoryParser, StoryParser>();
            container.EasyRegister<IStoryProvider, FilesystemStoryProvider>();
            container.EasyRegister<IResultListener, ConsoleResultListener>();
            container.EasyRegister<IFilesystem, Filesystem>();
            container.EasyRegister<IScenarioPreprocessor, ScenarioPreprocessor>();
            container.EasyRegister<ScenarioInterpreter>();
            container.EasyRegister<InterpreterForTypeFactory>();
            container.EasyRegister<ExtensionMethodHandler>();
            container.EasyRegister<IStoryToContextMapper, StoryToContextMapper>();
        }

        private void SetupCustomComponents(Container container, string[] args)
        {
            var command = args[0];

            if (command == "nunit")
            {
                container.EasyRegister<IFixtureGenerator, NUnitFixtureGenerator>();
                container.EasyRegister<NUnitTestMethodGenerator>();
                container.EasyRegister<CSharpMethodInvocationGenerator>();
                container.EasyRegister<IStoryHandler, FixtureGenerationStoryHandler>();
                container.EasyRegister<IStorEvilJob, StorEvilJob>();
                container.Register<ITestFixtureWriter> (new SingleFileTestFixtureWriter(args[3]));
            }
            else if (command == "execute")
            {
                container.EasyRegister<IStoryHandler, InPlaceRunner>();
                container.EasyRegister<IStorEvilJob, StorEvilJob>();

                if (args.Length > 1)
                    _settings.AssemblyLocations = new[] {args[1]};
                
                if (args.Length > 2)
                    _settings.StoryBasePath = args[2];
                else
                    _settings.StoryBasePath = Directory.GetCurrentDirectory();
            }
            else if (command == "help")
                container.Register<IStorEvilJob>(x => new DisplayHelpJob());

            else if (command == "setup")
                container.Register(x => GetSetupJob(args));
            else
                container.Register<IStorEvilJob>(x => new DisplayUsageJob());
        }

        private IStorEvilJob GetSetupJob(string[] args)
        {
            throw new NotImplementedException();
        }

        private StoryToContextMapper GetMapper(string[] args)
        {
            string pathToContextDll;

            if (args.Length > 1)
                pathToContextDll = args[1];
            else
                pathToContextDll = _configSource.GetConfig(Directory.GetCurrentDirectory()).AssemblyLocations.First();

            var mapper = new StoryToContextMapper();

            mapper.AddAssembly(pathToContextDll);
            return mapper;
        }
    }

    public class DisplayHelpJob : IStorEvilJob
    {
        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}