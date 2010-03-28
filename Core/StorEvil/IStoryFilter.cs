using System;
using System.Collections.Generic;
using System.Linq;
using StorEvil.Core;

namespace StorEvil
{
    public interface IStoryFilter
    {
        bool Include(Story story, IScenario scenario);
    }

    public class TagFilter : IStoryFilter
    {
        private readonly IEnumerable<string> _tags;

        public TagFilter(IEnumerable<string> tags)
        {
            _tags = tags;
            Console.WriteLine("Tags: ");
            Console.WriteLine(string.Join(",", _tags.ToArray()));
        }

        public bool Include(Story story, IScenario scenario)
        {
            if (_tags.Count() == 0)
                return true;

            return AnyTagMatches(story.Tags) || AnyTagMatches(scenario.Tags);
        }

        private bool AnyTagMatches(IEnumerable<string> tags)
        {
            return tags.Any(t => _tags.Contains(t));
        }
    }

    public class IncludeAllFilter : IStoryFilter
    {

        public bool Include(Story story, IScenario scenario)
        {
            return true;
        }
    }
}