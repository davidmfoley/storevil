using System.Collections.Generic;
using StorEvil.Core;

namespace StorEvil.Parsing
{
    public class StoryProvider : IStoryProvider
    {
        private readonly IStoryReader _reader;
        private readonly IStoryParser _parser;

        public StoryProvider(IStoryReader reader, IStoryParser parser)
        {
            _reader = reader;
            _parser = parser;
        }

        public IEnumerable<Story> GetStories()
        {
            Story story;
            var stories = new List<Story>();

            foreach (var storyInfo in _reader.GetStoryInfos())
            {
                if (null == (story = _parser.Parse(storyInfo.Text, storyInfo.Id)))
                    continue;

                story.Id = storyInfo.Id;
                stories.Add(story);
            }
            return stories;
        }
    }
}