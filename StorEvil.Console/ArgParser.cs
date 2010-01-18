using System;
using System.IO;
using Funq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Nunit;

namespace StorEvil.Console
{
    public class ArgParser
    {
        public IStorEvilJob ParseArguments(string[] args)
        {
            var container = new Container();

            SetupCommonComponents(container);
    
            SetupCustomComponents(container, args);

            return container.Resolve<IStorEvilJob>();
        }

        private void SetupCustomComponents(Container container, string[] args)
        {
            var command = args[0];
            if (command == "nunit")
                container.Register<IStorEvilJob>(x => GetNUnitJob(args));

            else if (command == "execute")
                container.Register<IStorEvilJob>(x => GetInPlaceJob(args));

            else if (command == "help")
                container.Register<IStorEvilJob>(x => new DisplayHelpJob());

            else if (command == "setup")
                container.Register<IStorEvilJob>(x => GetSetupJob(args));
            else
                container.Register <IStorEvilJob>(x => new DisplayUsageJob());
        }

        private void SetupCommonComponents(Container container)
        {
            
        }

        private IStorEvilJob GetSetupJob(string[] args)
        {
            throw new NotImplementedException();
        }

        private StorEvilJob GetInPlaceJob(string[] args)
        {
            string pathToContextDll = args[1];

            var mapper = new StoryToContextMapper();

            mapper.AddAssembly(pathToContextDll);

            var runner = new InPlaceRunner(new ConsoleResultListener(), new ScenarioPreprocessor());
            var storyBasePath = args.Length > 2 ? args[2] : Directory.GetCurrentDirectory();

            ConfigSettings settings = ConfigSettings.Default();
            var storyProvider = new FilesystemStoryProvider(storyBasePath, new StoryParser(), new Filesystem(), settings);

            return new StorEvilJob(storyProvider, mapper, runner);
        }

        private StorEvilJob GetNUnitJob(string[] args)
        {
            string pathToContextDll = args[1];
            string storyBasePath = args[2];
            string outputFile = args[3];

            ConfigSettings settings = ConfigSettings.Default();

            var storyProvider = new FilesystemStoryProvider(storyBasePath, new StoryParser(), new Filesystem(), settings);
            
            var mapper = new StoryToContextMapper();
            mapper.AddAssembly(pathToContextDll);

            var handler = new FixtureGenerationStoryHandler(
                BuildFixtureGenerator(),
                new SingleFileTestFixtureWriter(outputFile));

            return new StorEvilJob(
                storyProvider,
                mapper,
                handler);
        }

        private NUnitFixtureGenerator BuildFixtureGenerator()
        {
            //TODO: cleanup
            return new NUnitFixtureGenerator(new ScenarioPreprocessor(),
                                             new NUnitTestMethodGenerator(
                                                 new CSharpMethodInvocationGenerator(
                                                     new ScenarioInterpreter(
                                                         new InterpreterForTypeFactory(new ExtensionMethodHandler())))));
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