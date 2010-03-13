using System;
using System.Collections.Generic;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.ResultListeners
{
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

        public void CouldNotInterpret(CouldNotInterpretInfo couldNotInterpretInfo)
        {
            AllListeners(x => x.CouldNotInterpret(couldNotInterpretInfo));
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