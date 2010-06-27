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
 
        public InPlaceStoryRunner(IResultListener listener,
                                  IScenarioPreprocessor preprocessor,
                                  ScenarioInterpreter scenarioInterpreter,
                                  IStoryFilter filter,
                                  ISessionContext context)
              : base(listener, preprocessor, filter, context)
        {
           
            _scenarioRunner = new InPlaceScenarioRunner(listener, scenarioInterpreter);
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

            Result.Failed = Result.Failed + failed;
        }
    }
}