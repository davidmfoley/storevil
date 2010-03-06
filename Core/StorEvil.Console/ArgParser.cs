using System;
using System.Collections.Generic;
using System.IO;
using Funq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Core.Configuration;
using StorEvil.InPlace;
using StorEvil.Parsing;
using StorEvil.ResultListeners;

namespace StorEvil.Console
{
    public class ArgParser
    {
        private readonly IConfigSource _configSource;
        private ConfigSettings _settings;
        public Container Container { get; private set; }

        private const string StandardHelpText =
            @"
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

            Container = new Container();
        }

        public IStorEvilJob ParseArguments(string[] args)
        {
            ParseCommonConfigSettings(Container, args);
            SetupCommonComponents(Container);
            SetupCustomComponents(Container, args);

            return Container.Resolve<IStorEvilJob>();
        }

        private void ParseCommonConfigSettings(Container container, string[] args)
        {
            SwitchParser<ConfigSettings> switchParser = new CommonSwitchParser();

            _settings = _configSource.GetConfig(Directory.GetCurrentDirectory());
            _settings.StoryBasePath = Directory.GetCurrentDirectory();
            switchParser.Parse(args, _settings);

            container.Register(_settings);
        }

        private void SetupCommonComponents(Container container)
        {
            container.EasyRegister<IStoryParser, StoryParser>();
            container.EasyRegister<IStoryProvider, StoryProvider>();
            container.EasyRegister<IStoryReader, FilesystemStoryReader>();
            container.Register(GetResultListener());
            container.EasyRegister<IFilesystem, Filesystem>();
            container.EasyRegister<IScenarioPreprocessor, ScenarioPreprocessor>();
            container.EasyRegister<ScenarioInterpreter>();
            container.EasyRegister<InterpreterForTypeFactory>();
            container.EasyRegister<ExtensionMethodHandler>();

            container.Register<IStoryToContextMapper>(GetStoryToContextMapper());
        }

        private IResultListener GetResultListener()
        {
            var compositeListener = new CompositeListener();

            if (!_settings.Quiet)
            {
                compositeListener.AddListener(new ConsoleResultListener
                                                  {
                                                      ColorEnabled = _settings.ConsoleMode == ConsoleMode.Color
                                                  });
            }
            if (!string.IsNullOrEmpty(_settings.OutputFileFormat))
            {        
                string outputFile;
                if (!string.IsNullOrEmpty(_settings.OutputFile))
                    outputFile = _settings.OutputFile;
                else
                    outputFile = "storevil-output." + _settings.OutputFileFormat.ToLower();

                if (_settings.OutputFileFormat.ToLower() == "xml")
                {
                    compositeListener.AddListener(new XmlReportListener(new FileWriter(outputFile, true)));
                }
            }

            return compositeListener;
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
                container.Register<IStorEvilJob>(
                    new DisplayHelpJob(GetStandardHelpText() + "\r\n\r\nSwitches for '" + args[1] + "': \r\n\r\n" +
                                       helpJobFactory.GetUsage()));
            else
                container.Register<IStorEvilJob>(new DisplayHelpJob(GetStandardHelpText()));
        }

        private string GetStandardHelpText()
        {
            return StandardHelpText + "\r\n\r\nGeneral switches:\r\n\r\n" + new CommonSwitchParser().GetUsage();
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

    public class CompositeListener : IResultListener
    {
        public List<IResultListener> Listeners = new List<IResultListener>();

        public void AddListener(IResultListener listener)
        {
            Listeners.Add(listener);
        }

        private void AllListeners(Action<IResultListener> action)
        {
            foreach (var listener in Listeners)
            {
                action(listener);
            }
        }

        public void StoryStarting(Story story)
        {
            AllListeners(x => x.StoryStarting(story));
        }

        public void ScenarioStarting(Scenario scenario)
        {
            AllListeners(x => x.ScenarioStarting(scenario));
        }

        public void ScenarioFailed(Scenario scenario, string successPart, string failedPart, string message)
        {
            AllListeners(x => x.ScenarioFailed(scenario, successPart, failedPart, message));
        }

        public void CouldNotInterpret(Scenario scenario, string line)
        {
            AllListeners(x => x.CouldNotInterpret(scenario, line));
        }

        public void Success(Scenario scenario, string line)
        {
            AllListeners(x => x.Success(scenario, line));
        }

        public void ScenarioSucceeded(Scenario scenario)
        {
            AllListeners(x => x.ScenarioSucceeded(scenario));
        }

        public void Finished()
        {
            AllListeners(x => x.Finished());
        }
    }

    internal class CommonSwitchParser : SwitchParser<ConfigSettings>
    {
        public CommonSwitchParser()
        {
            AddSwitch("--story-path", "-p")
                .SetsField(s => s.StoryBasePath)
                .WithDescription(
                    "Sets the base path used when searching for story files.\r\nIf not set, the current working directory is assumed.");

            AddSwitch("--assemblies", "-a")
                .SetsField(s => s.AssemblyLocations)
                .WithDescription(
                    "Sets the location (relative to current path) of the context assemblies used to parse the stories.");

            AddSwitch("--output-file", "-o")
                .SetsField(s => s.OutputFile)
                .WithDescription("If set, storevil will output to the specified file.");

            AddSwitch("--output-file-format", "-f")
                .SetsField(s => s.OutputFileFormat)
                .WithDescription(
                    "Sets the format of output to the file specified by --output-file (ONLY xml is supported so far)\r\n" +
                    "If nothing is specified, the output file location will be: storevil.output.{format}");

            AddSwitch("--console-mode", "-c")
                .SetsEnumField(s => s.ConsoleMode)  
                .WithDescription("Sets the format of output to the console (color, nocolor, quiet)");
        }
    }
}