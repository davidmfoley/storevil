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
        private readonly ConfigSettings _settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="basePath">the base path for retrieving the stories</param>
        /// <param name="parser">the parser that will parse the text we find in the stories</param>
        /// <param name="filesystem"></param>
        /// <param name="settings"></param>
        public FilesystemStoryProvider(IStoryParser parser, IFilesystem filesystem, ConfigSettings settings)
        {
            _settings = settings;
            //_settings = settings;
            BasePath = settings.StoryBasePath;
            Parser = parser;
            Filesystem = filesystem;
        }

        public string BasePath { get; set; }
        public IStoryParser Parser { get; set; }
        public IFilesystem Filesystem { get; set; }

        public IEnumerable<Story> GetStories()
        {
            return GetStoriesRecursive(BasePath);
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
            if (_settings.ScenarioExtensions == null || ! _settings.ScenarioExtensions.Any())
                return true;

            string extension = Path.GetExtension(file);
            return _settings.ScenarioExtensions.Any(x => extension == x);
        }
    }
}