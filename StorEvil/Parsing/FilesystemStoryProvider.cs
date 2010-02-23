using System.Collections.Generic;
using System.IO;
using System.Linq;
using StorEvil.Core;

namespace StorEvil
{
    /// <summary>
    /// returns all of the stories/specs from a directory
    /// </summary>
    public class FilesystemStoryProvider : IStoryProvider
    {
        private readonly ConfigSettings Settings;

        public FilesystemStoryProvider(IStoryParser parser, IFilesystem filesystem, ConfigSettings settings)
        {
            Settings = settings;
            Parser = parser;
            Filesystem = filesystem;
        }

        public IStoryParser Parser { get; set; }
        public IFilesystem Filesystem { get; set; }

        public IEnumerable<Story> GetStories()
        {
            return GetStoriesRecursive(Settings.StoryBasePath);
        }

        public IEnumerable<Story> GetStoriesRecursive(string path)
        {
            Story story;
            var stories = new List<Story>();

            foreach (var file in Filesystem.GetFilesInFolder(path))
            {
                if (!ExtensionIsSupportedByCurrentSettings(file))
                    continue;

                if (null == (story = Parser.Parse(Filesystem.GetFileText(file))))
                    continue;

                story.Id = Path.GetFileNameWithoutExtension(file);
                stories.Add(story);
            }

            foreach (var subPath in Filesystem.GetSubFolders(path))
            {
                stories.AddRange(GetStoriesRecursive(subPath));
            }
            return stories;
        }

        private bool ExtensionIsSupportedByCurrentSettings(string file)
        {
            if (Settings.ScenarioExtensions == null || ! Settings.ScenarioExtensions.Any())
                return true;

            var extension = Path.GetExtension(file);
            return Settings.ScenarioExtensions.Any(x => extension == x);
        }
    }
}