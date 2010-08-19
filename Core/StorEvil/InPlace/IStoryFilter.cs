using System.Collections.Generic;
using System.Linq;
using StorEvil.Core;
using StorEvil.Interpreter;

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
            _tags = tags ?? new string[0];           
        }

        public bool Include(Story story, IScenario scenario)
        {
            DebugTrace.Trace(this, "story: " + story.Location );
            DebugTrace.Trace(this, "scenario: " + scenario.Name);
            if (_tags.Count() == 0)
                return true;

            bool include = AnyTagMatches(story.Tags) || AnyTagMatches(scenario.Tags);

            DebugTrace.Trace(this, "include: " + include);

            return include;
        }

        private bool AnyTagMatches(IEnumerable<string> tags)
        {
            return (tags ?? new string[0]).Any(t => _tags.Contains(t));
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