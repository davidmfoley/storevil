using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;

namespace StorEvil.InPlace
{
    public class InPlaceScenarioRunner
    {
        private readonly IResultListener _listener;
        private readonly ScenarioInterpreter _scenarioInterpreter;

        private readonly ScenarioLineExecuter _lineExecuter;

        public InPlaceScenarioRunner(IResultListener listener, MemberInvoker memberInvoker,
                                     ScenarioInterpreter scenarioInterpreter)
        {
            _listener = listener;
            _scenarioInterpreter = scenarioInterpreter;
            _lineExecuter = new ScenarioLineExecuter(memberInvoker, scenarioInterpreter, listener);
        }

        public bool ExecuteScenario(Scenario scenario, ScenarioContext storyContext)
        {
            _listener.ScenarioStarting(scenario);
            _scenarioInterpreter.NewScenario();

            foreach (var line in scenario.Body)
            {
                LineStatus status = _lineExecuter.ExecuteLine(scenario, storyContext, line.Text);
                if (LineStatus.Failed == status)
                    return false;
                if (LineStatus.Pending ==status)
                    return true;
            }

            _listener.ScenarioSucceeded(scenario);
            return true;
        }
    }
}