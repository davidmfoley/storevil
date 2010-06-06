using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.Parsing;
using StorEvil.Resharper.Elements;

namespace StorEvil.Resharper.Provider
{
    class StorEvilAssemblyExplorer
    {
        private StorEvilTestProvider _provider;
        private readonly StorEvilTestEnvironment _environment;
        private readonly StorEvilResharperConfigProvider _configProvider;

        public StorEvilAssemblyExplorer(StorEvilTestProvider provider, StorEvilTestEnvironment environment)
        {
            _provider = provider;
            _environment = environment;           
        }

        public void ExploreProject(IProject project, UnitTestElementConsumer consumer)
        {
            var config = _environment.GetProject(project).ConfigSettings;

            var projectElement = new StorEvilProjectElement(_provider, null, project, project.Name, config.AssemblyLocations);
            consumer(projectElement);

            if (config == null || config.StoryBasePath == null)
                return;

            var stories = GetStoriesForProject(config);

            foreach (Story story in stories)
                AddStoryElement(config, story, project, consumer, projectElement);
        }

        private IEnumerable<Story> GetStoriesForProject(ConfigSettings config)
        {
            var filesystemStoryReader = new FilesystemStoryReader(new Filesystem(), config);
            var storyProvider = new StoryProvider(filesystemStoryReader, new StoryParser());

            return storyProvider.GetStories();
        }

        private void AddStoryElement(ConfigSettings config, Story story, IProject project,
                                     UnitTestElementConsumer consumer, StorEvilProjectElement parent)
        {
            var storyElement = GetStoryElement(parent, project, story, config);
            consumer(storyElement);

            foreach (IScenario scenario in story.Scenarios)
                AddScenarioElement(project, consumer, storyElement, scenario);
        }

        private StorEvilStoryElement GetStoryElement(StorEvilProjectElement parent, IProject project, Story story, ConfigSettings config)
        {
            return new StorEvilStoryElement(_provider, parent, project, story.Summary, config, story.Id);
        }

        private void AddScenarioElement(IProject project, UnitTestElementConsumer consumer,
                                        StorEvilStoryElement storyElement, IScenario scenario)
        {
            if (scenario is Scenario)
                consumer(new StorEvilScenarioElement(_provider, storyElement, project, scenario.Name, (Scenario)scenario));
            else
                consumer(new StorEvilScenarioOutlineElement(_provider, storyElement, project, scenario.Name,
                                                            (ScenarioOutline)scenario));
        }
    }
}