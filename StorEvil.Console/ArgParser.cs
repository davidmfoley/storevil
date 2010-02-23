using System.IO;
using Funq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Core.Configuration;
using StorEvil.InPlace;

namespace StorEvil.Console
{
    public class ArgParser
    {
        private readonly IConfigSource _configSource;
        private ConfigSettings _settings;
        private readonly Container _container;
        private const string StandardHelpText = @"
StorEvil is a natural language BDD framework and runner.
  
Available commands:

 execute - Executes the specs in the console
 nunit   - Generates NUnit text fixtures

usage:
  'storevil {command} {switches}  - to execute the command
  'storevil help {command}'       - for more information about usage of a command";

        public ArgParser(IConfigSource source)
        {
            _configSource = source;
          
            _container = new Container();
        }

        public IStorEvilJob ParseArguments(string[] args)
        {
            ParseCommonConfigSettings(_container, args);
            SetupCommonComponents(_container);
            SetupCustomComponents(_container, args);

            return _container.Resolve<IStorEvilJob>();
        }

        private void SetupCommonComponents(Container container)
        {
            

           
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
                mapper.AddAssembly(location);

            container.Register<IStoryToContextMapper>(mapper);
        }

        private void ParseCommonConfigSettings(Container container, string[] args)
        {
            SwitchParser<ConfigSettings> switchParser = GetSwitchParser();

            _settings = _configSource.GetConfig(Directory.GetCurrentDirectory());
            
            _settings.StoryBasePath = Directory.GetCurrentDirectory();

            switchParser.Parse(args, _settings);

            container.Register(_settings);
        }

        private SwitchParser<ConfigSettings> GetSwitchParser()
        {
            var switchParser = new SwitchParser<ConfigSettings>();

            switchParser
                .AddSwitch("--story-path", "-p")
                .SetsField(s => s.StoryBasePath)
                .WithDescription("Sets the base path used when searching for story files.\r\nIf not set, the current working directory is assumed.");

            switchParser.AddSwitch("--assemblies", "-a")
                .SetsField(s => s.AssemblyLocations)
                .WithDescription("Sets the location (relative to current path) of the context assemblies used to parse the stories.");
            return switchParser;
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
            if (args.Length <= 1)
            {
                container.Register<IStorEvilJob>(new DisplayHelpJob(GetStandardHelpText()));
                return;
            }
            var helpJobFactory = GetJobFactory(args[1]);

            if (helpJobFactory != null)
                container.Register<IStorEvilJob>(new DisplayHelpJob(GetStandardHelpText() + "\r\n\r\nSwitches for '" + args[1]+ "': \r\n\r\n" + helpJobFactory.GetUsage()));
            else
                container.Register<IStorEvilJob>(new DisplayHelpJob(GetStandardHelpText()));
        }

        private string GetStandardHelpText()
        {

            return StandardHelpText + "\r\n\r\nGeneral switches:\r\n\r\n" + GetSwitchParser().GetUsage()    ;
        }

        private IJobFactory GetJobFactory(string command)
        {
            IJobFactory jobFactory = null;

            if (command == "nunit")
                jobFactory = new NUnitJobFactory();
            else if (command == "execute")
                jobFactory = new InPlaceJobFactory();
            
            return jobFactory;
        }
    }
}