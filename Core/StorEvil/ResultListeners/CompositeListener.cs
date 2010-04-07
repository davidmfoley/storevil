using System;
using System.Collections.Generic;
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

    public class CompositeListener : IResultListener
    {
        public List<IResultListener> Listeners = new List<IResultListener>();

        public void AddListener(IResultListener listener)
        {
            Listeners.Add(listener);
        }

        private void AllListeners(Action<IResultListener> action)
        {
            foreach (var listener in Listeners)
            {
                action(listener);
            }
        }

        public void StoryStarting(Story story)
        {
            AllListeners(x => x.StoryStarting(story));
        }

        public void ScenarioStarting(Scenario scenario)
        {
            AllListeners(x => x.ScenarioStarting(scenario));
        }

        public void ScenarioFailed(ScenarioFailureInfo scenarioFailureInfo)
        {
            AllListeners(x => x.ScenarioFailed(scenarioFailureInfo));
        }

        public void ScenarioPending(ScenarioPendingInfo scenarioPendingInfo)
        {
            AllListeners(x => x.ScenarioPending(scenarioPendingInfo));
        }

        public void Success(Scenario scenario, string line)
        {
            AllListeners(x => x.Success(scenario, line));
        }

        public void ScenarioSucceeded(Scenario scenario)
        {
            AllListeners(x => x.ScenarioSucceeded(scenario));
        }

        public void Finished()
        {
            AllListeners(x => x.Finished());
        }
    }
}