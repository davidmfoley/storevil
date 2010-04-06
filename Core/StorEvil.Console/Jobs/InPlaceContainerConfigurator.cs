using System;
using System.Collections.Generic;
using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.ResultListeners;
using StorEvil.Utility;

namespace StorEvil.Console
{
    public class InPlaceContainerConfigurator : ContainerConfigurator<InPlaceSettings>
    {
        protected override void SetupCustomComponents(Container container)
        {
            container.EasyRegister<IStoryHandler, InPlaceStoryRunner>();
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
            container.Register<IStoryFilter>(new TagFilter(CustomSettings.Tags ?? new string[0]));
        }

        protected override void SetupSwitches(SwitchParser<InPlaceSettings> parser)
        {
            parser.AddSwitch("--tags", "-g")
                .SetsField(x => x.Tags);
        }
    }

    public class InPlaceDebugContainerConfigurator : ContainerConfigurator<InPlaceSettings>
    {
        protected override void SetupCustomComponents(Container container)
        {
            container.EasyRegister<IStorEvilJob, StorEvilDebugJob>();
            container.Register<IStoryFilter>(new TagFilter(CustomSettings.Tags ?? new string[0]));
        }

        protected override void SetupSwitches(SwitchParser<InPlaceSettings> parser)
        {
            parser.AddSwitch("--tags", "-g")
                .SetsField(x => x.Tags);
        }
    }

    public class StorEvilDebugJob : IStorEvilJob
    {
        private readonly IStoryProvider _storyProvider;
        private readonly AssemblyGenerator _generator;
        private readonly ConfigSettings _configSettings;
        private readonly IFilesystem _filesystem;

        JobResult Result = new JobResult();

        public StorEvilDebugJob(IStoryProvider storyProvider, AssemblyGenerator generator, ConfigSettings configSettings, IFilesystem filesystem)
        {
            _storyProvider = storyProvider;
            _generator = generator;
            _configSettings = configSettings;
            _filesystem = filesystem;
        }

        public int Run()
        {
            var stories = _storyProvider.GetStories();
            foreach (var story in stories)
            {
                CompileAndRunInSeparateAppDomain(story);
            }

            return 0;
        }

        private void CompileAndRunInSeparateAppDomain(Story story)
        {
            var assemblyLocation = BuildAssembly(story);
            try
            {
               Result += ExecuteInSeparateAppDomain(assemblyLocation, story);
            }
            finally
            {
                _filesystem.Delete(assemblyLocation);
            }
            throw new NotImplementedException();
        }

        private JobResult ExecuteInSeparateAppDomain(string assemblyLocation, Story story)
        {
            // Construct and initialize settings for a second AppDomain.
            var ads = new AppDomainSetup
                          {
                              ApplicationBase = System.Environment.CurrentDirectory,
                              DisallowBindingRedirects = false,
                              DisallowCodeDownload = true,
                              ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
                          };

            AppDomain testDomain = AppDomain.CreateDomain("TestDomain", null, ads);

            try
            {
                var handler = testDomain.CreateInstanceFrom(
                    assemblyLocation,
                    "StorEvilTestAssembly.StorEvilDriver").Unwrap() as IStoryHandler;

                handler.HandleStory(story);
                handler.Finished();

                return handler.GetResult();
            }

            finally
            {
                AppDomain.Unload(testDomain);
            }
        }

        private string BuildAssembly(Story story)
        {
            return _generator.GenerateAssembly(story, _configSettings.AssemblyLocations);
        }
    }
}