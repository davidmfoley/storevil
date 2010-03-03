using System.Collections.Generic;
using StorEvil.Core;

namespace StorEvil.Resharper
{
    internal class SingleScenarioStoryProvider : IStoryProvider
    {
        private readonly IScenario _scenario;

        public SingleScenarioStoryProvider(IScenario scenario)
        {
            _scenario = scenario;
        }

        public IEnumerable<Story> GetStories()
        {
            return new[] {new Story(_scenario.Id, _scenario.Name, new[] {_scenario})};
        }
    }
}