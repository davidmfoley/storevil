using System;
using System.Linq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Interpreter;

namespace StorEvil.InPlace
{
    public class InPlaceScenarioRunner
    {
        private readonly IEventBus _eventBus;
        private readonly ScenarioInterpreter _scenarioInterpreter;
        private readonly ScenarioLineExecuter _lineExecuter;
        private string _exceptionInfo;

        public InPlaceScenarioRunner(IEventBus eventBus, ScenarioInterpreter scenarioInterpreter)
        {
            _eventBus = eventBus;
            _scenarioInterpreter = scenarioInterpreter;
            _lineExecuter = new ScenarioLineExecuter(scenarioInterpreter, eventBus);
        }

        public bool ExecuteScenario(Scenario scenario, ScenarioContext scenarioContext)
        {
            _eventBus.Raise(new ScenarioStarting { Scenario = scenario});
            //_scenarioInterpreter.NewScenario();

            var result = ExecutionStatus.Passed;

            foreach (var line in (scenario.Background ?? new ScenarioLine[0]).Union(scenario.Body))
            {
                LineStatus status = _lineExecuter.ExecuteLine(scenario, scenarioContext, line.Text);
                if (LineStatus.Failed == status)
                {
                    result = ExecutionStatus.Failed;
                }
                if (LineStatus.Pending == status)
                {
                    result = ExecutionStatus.Pending;
                }
            }

            if (!TryToDisposeScenarioContext(scenarioContext))
            {
                result = ExecutionStatus.Failed;
            }

            switch (result)
            {
                case ExecutionStatus.Failed:
                {
                    _eventBus.Raise(new ScenarioFailed { Scenario = scenario, ExceptionInfo = _exceptionInfo});
                    return false;
                }
                case ExecutionStatus.Pending:
                {
                    _eventBus.Raise(new ScenarioPending { Scenario = scenario });
                    return true;
                }
                default:
                {
                    _eventBus.Raise(new ScenarioPassed { Scenario = scenario });
                    return true;
                }
            }
        }

        private bool TryToDisposeScenarioContext(ScenarioContext scenarioContext)
        {
            try
            {
                scenarioContext.Dispose();
                return true;
            }
            catch (Exception e)
            {
                _exceptionInfo = e.ToString();
                return false;
            }
        }

        private enum ExecutionStatus
        {
            Passed,
            Failed,
            Pending
        }

    }

    
}