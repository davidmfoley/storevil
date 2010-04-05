using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public class InPlaceCompilingStoryRunner : IStoryHandler
    {
        private readonly IResultListener _listener;

        private readonly IStoryFilter _filter;
        private readonly MemberInvoker _memberInvoker;

        private readonly IScenarioPreprocessor _preprocessor;
        private readonly ScenarioInterpreter _scenarioInterpreter;
        private readonly AssemblyGenerator _generator = new AssemblyGenerator();

        public InPlaceCompilingStoryRunner(IResultListener listener,
                                           IScenarioPreprocessor preprocessor,
                                           ScenarioInterpreter scenarioInterpreter,
                                           IStoryFilter filter)
        {
            _listener = listener;
            _preprocessor = preprocessor;
            _scenarioInterpreter = scenarioInterpreter;
            _filter = filter;

            _memberInvoker = new MemberInvoker();
        }

        public int HandleStory(Story story, StoryContext context)
        {
            var scenarios = GetScenariosMatchingFilter(story).ToArray();

            var assembly = _generator.GenerateAssembly(story, scenarios);

            
            _listener.StoryStarting(story);

            return ExecuteAssemblyDriver(assembly, story, context, scenarios);

        }

        private int ExecuteAssemblyDriver(Assembly assembly, Story story, StoryContext context, Scenario[] scenarios)
        {
            var preprocessor = new ScenarioPreprocessor();
          
            var driverType = GetDriverType(assembly);
            var driver = Activator.CreateInstance(driverType,
                                                  _listener,
                                                  _memberInvoker,
                                                  _scenarioInterpreter,
                                                  scenarios
                );

            var methodInfo = driverType.GetMethod("Execute");

            return (int)methodInfo.Invoke(driver, new[] { context });
        }

        private Type GetDriverType(Assembly assembly)
        {
            return assembly.GetTypes().First();
        }

        private IEnumerable<Scenario> GetScenariosMatchingFilter(Story story)
        {
            return GetScenarios(story).Where(s => _filter.Include(story, s));
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