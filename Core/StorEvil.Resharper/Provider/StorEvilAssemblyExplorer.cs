using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.Parsing;
using StorEvil.Resharper.Elements;
using StorEvil.Utility;

namespace StorEvil.Resharper.Provider
{
    class StorEvilAssemblyExplorer
    {
        private StorEvilTestProvider _provider;
        private readonly StorEvilTestEnvironment _environment;


        public StorEvilAssemblyExplorer(StorEvilTestProvider provider, StorEvilTestEnvironment environment)
        {
            _provider = provider;
            _environment = environment;           
        }

        public void ExploreProject(IProject project, UnitTestElementConsumer consumer)
        {
            var config = _environment.GetProject(project.ProjectFile.Location.FullPath).ConfigSettings;

            if (config.StoryBasePath == null)
                return;

            var stories = GetStoriesForProject(config);

            var projectElement = new StorEvilProjectElement(_provider, null, project, project.Name, config.AssemblyLocations);
            consumer(projectElement);

            

            AddStoriesToProject(project, consumer, projectElement, stories);
        }

        private void AddStoriesToProject(IProject project, UnitTestElementConsumer consumer, StorEvilProjectElement projectElement, IEnumerable<Story> stories)
        {
            var folders = new Dictionary<string, UnitTestElement>();
            var root = project.ParentFolder.Name;

            foreach (Story story in stories)
            {
                var relatvePath = PathHelper.GetRelativePathPieces(root, story.Location);
                AddStoryElement(story, project, consumer, projectElement);
            }
        }

        private IEnumerable<Story> GetStoriesForProject(ConfigSettings config)
        {
            var filesystemStoryReader = new FilesystemStoryReader(new Filesystem(), config);
            var storyProvider = new StoryProvider(filesystemStoryReader, new StoryParser());

            return storyProvider.GetStories();
        }

        private void AddStoryElement(Story story, IProject project,
                                     UnitTestElementConsumer consumer, StorEvilProjectElement parent)
        {
            var storyElement = GetStoryElement(parent, project, story);
            consumer(storyElement);

            foreach (IScenario scenario in story.Scenarios)
                AddScenarioElement(project, consumer, storyElement, scenario);
        }

        private StorEvilStoryElement GetStoryElement(UnitTestElement parent, IProject project, Story story)
        {
            return new StorEvilStoryElement(_provider, parent, project, story.Summary, story.Location);
        }

        private void AddScenarioElement(IProject project, UnitTestElementConsumer consumer,
                                        StorEvilStoryElement storyElement, IScenario scenario)
        {
            if (scenario is Scenario)
                consumer(new StorEvilScenarioElement(_provider, storyElement, project, scenario.Name, (Scenario)scenario));
            else
            {
                var outline = (ScenarioOutline) scenario;
                var outlineElement = new StorEvilScenarioOutlineElement(_provider, storyElement, project, scenario.Name,
                                                                                        outline);
                consumer(outlineElement);
                var i = 0;
                foreach (var child in new ScenarioPreprocessor().Preprocess(scenario))
                {                 
                    consumer(new StorEvilScenarioElement(_provider, outlineElement, project, BuildExampleRowScenarioName(outline, i), child));
                    i++;
                }
            }
        }

        private string BuildExampleRowScenarioName(ScenarioOutline outline, int index)
        {
            var examples = outline.Examples[index];
            var nameParts = new string[examples.Length];
           
            for (int i = 0; i < examples.Length; i++)
            {
                nameParts[i] = outline.FieldNames.ElementAt(i) + "=" + examples[i];
            }
            return string.Join(", ", nameParts);
            
        }
    }


}