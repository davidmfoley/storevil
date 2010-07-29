using System;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.ResultListeners
{
    public class RemoteListener : MarshalByRefObject, IResultListener
    {
        private IResultListener _inner;
        public void StoryStarting(Story story)
        {
            _inner.StoryStarting(story);
        }

        public void ScenarioStarting(Scenario scenario)
        {
            _inner.ScenarioStarting(scenario);
        }

        public void ScenarioFailed(ScenarioFailureInfo scenarioFailureInfo)
        {
            _inner.ScenarioFailed(scenarioFailureInfo);
        }

        public void ScenarioPending(ScenarioPendingInfo scenarioPendingInfo)
        {
            _inner.ScenarioPending(scenarioPendingInfo);
        }

        public void Success(Scenario scenario, string line)
        {
            _inner.Success(scenario, line);
        }

        public void ScenarioSucceeded(Scenario scenario)
        {
            _inner.ScenarioSucceeded(scenario);
        }

        public void Finished()
        {
            _inner.Finished();
        }

        public RemoteListener(IResultListener inner)
        {
            _inner = inner;
        } 
    }
}