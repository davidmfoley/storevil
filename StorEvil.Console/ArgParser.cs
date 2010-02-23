using System;
using System.IO;
using Funq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.Console
{
    public class ArgParser
    {
        private readonly IConfigSource _configSource;
        private readonly ConfigSettings _settings;
        private readonly Container _container;
        private const string StandardHelpText = @"
StorEvil is a natural language BDD framework and runner.
  
Available commands:

 execute - Executes the specs in the console
 nunit   - Generates NUnit text fixtures

usage:
  'storevil {command}          - to execute the command
  'storevil help {command}'    - for more information about usage of a command";

        public ArgParser(IConfigSource source)
        {
            _configSource = source;
            _settings = source.GetConfig(Directory.GetCurrentDirectory());
            _settings.StoryBasePath = Directory.GetCurrentDirectory();
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
            container.Register(_settings);

            container.EasyRegister<IStoryParser, StoryParser>();
            container.EasyRegister<IStoryProvider, FilesystemStoryProvider>();
            container.EasyRegister<IResultListener, ConsoleResultListener>();
            container.EasyRegister<IFilesystem, Filesystem>();
            container.EasyRegister<IScenarioPreprocessor, ScenarioPreprocessor>();
            container.EasyRegister<ScenarioInterpreter>();
            container.EasyRegister<InterpreterForTypeFactory>();
            container.EasyRegister<ExtensionMethodHandler>();

            var mapper = new StoryToContextMapper();
            foreach (var location in _settings.AssemblyLocations)
            {
                mapper.AddAssembly(location);
            }

            container.Register<IStoryToContextMapper>(mapper);
        }

        private void SetupCustomComponents(Container container, string[] args)
        {
            
            if (args.Length == 0 || args[0] == "help")
            {
                SetupHelpJob(args, container);
                return;
            }

            var command = args[0];

            var jobFactory = GetJobFactory(command);
            if (jobFactory == null)
                SetupHelpJob(args, container);  
            else
                jobFactory.SetupContainer(container, args);
        }

        private void SetupHelpJob(string[] args, Container container)
        {
            if (args.Length <=   1)
            {
                container.Register<IStorEvilJob>(new DisplayHelpJob(StandardHelpText));
                return;
            }
            var helpJobFactory = GetJobFactory(args[1]);

            if (helpJobFactory != null)
                container.Register<IStorEvilJob>(new DisplayHelpJob("Usage for '" + args[1]+ "': \r\n " + helpJobFactory.GetUsage()));
            else
                container.Register<IStorEvilJob>(new DisplayHelpJob(StandardHelpText));
        }

        private IJobFactory GetJobFactory(string command)
        {
            IJobFactory jobFactory = null;

            if (command == "nunit")
            {
                jobFactory = new NUnitJobFactory();
            }
            else if (command == "execute")
            {
                jobFactory = new InPlaceJobFactory();
            }
            return jobFactory;
        }

        private IStorEvilJob GetSetupJob(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}