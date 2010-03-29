using System.Collections.Generic;
using System.Linq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public class InPlaceStoryRunner : IStoryHandler
    {
        private readonly IResultListener _listener;

        private readonly ScenarioInterpreter _scenarioInterpreter;
        private readonly IStoryFilter _filter;
        private readonly MemberInvoker _memberInvoker;

        private readonly IScenarioPreprocessor _preprocessor;
        private readonly InPlaceScenarioRunner _scenarioRunner;

        public InPlaceStoryRunner(IResultListener listener, 
                            IScenarioPreprocessor preprocessor,
                            ScenarioInterpreter scenarioInterpreter,
                            IStoryFilter filter)
        {
            _listener = listener;
            _preprocessor = preprocessor;
            _scenarioInterpreter = scenarioInterpreter;
            _filter = filter;

            _memberInvoker = new MemberInvoker();  
            _scenarioRunner = new InPlaceScenarioRunner(_listener, _memberInvoker, scenarioInterpreter);

                      
        }

        public int HandleStory(Story story, StoryContext context)
        {
            int failed = 0;
            _listener.StoryStarting(story);
            foreach (var scenario in GetScenarios(story))
            {
                if (!_filter.Include(story, scenario))
                    continue;
                _listener.ScenarioStarting(scenario);
                _scenarioInterpreter.NewScenario();

                using (var scenarioContext = context.GetScenarioContext())
                {
                    if (!_scenarioRunner.ExecuteScenario(scenario, scenarioContext))
                        failed++;
                }
            }
            return failed;
        }

        private IEnumerable<Scenario> GetScenarios(Story story)
        {
            return story.Scenarios.SelectMany(scenario => _preprocessor.Preprocess(scenario));
        }

        public void Finished()
        {
            _listener.Finished();
        }

    }
}