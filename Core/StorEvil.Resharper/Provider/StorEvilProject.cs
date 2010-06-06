using System.Collections.Generic;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.Parsing;

namespace StorEvil.Resharper
{
    internal class StorEvilProject
    {
        public ConfigSettings ConfigSettings { get; private set;} 

        public StorEvilProject(ConfigSettings configSettings)
        {
            ConfigSettings = configSettings;           
        }

        public IEnumerable<Story> GetStories(string location)
        {
            var storyReader = new SingleFileStoryReader(new Filesystem(), ConfigSettings, location);
            var storyProvider = new StoryProvider(storyReader, new StoryParser());
            return storyProvider.GetStories();
        }
    }
}