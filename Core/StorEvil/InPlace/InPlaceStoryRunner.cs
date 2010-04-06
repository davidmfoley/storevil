using System.Collections.Generic;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public class InPlaceStoryRunner :InPlaceStoryRunnerBase
    {
          private readonly InPlaceScenarioRunner _scenarioRunner;
        private int _failed;

        public InPlaceStoryRunner(IResultListener listener,
                                  IScenarioPreprocessor preprocessor,
                                  ScenarioInterpreter scenarioInterpreter,
                                  IStoryFilter filter,
                                  IStoryContextFactory contextFactory)
              : base(listener, preprocessor, filter, contextFactory)
        {
           
            _scenarioRunner = new InPlaceScenarioRunner(listener,new MemberInvoker(), scenarioInterpreter);
        }

        protected override void Execute(Story story, IEnumerable<Scenario> scenariosMatchingFilter, StoryContext context)
        {
            int failed = 0;
            foreach (var scenario in scenariosMatchingFilter)
            {
                using (var scenarioContext = context.GetScenarioContext())
                {
                    if (!_scenarioRunner.ExecuteScenario(scenario, scenarioContext))
                        failed++;
                }
            }
            _failed += failed;
        }
    }
}