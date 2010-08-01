using System;
using System.Collections.Generic;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.ResultListeners
{
    public class CompositeListener : AutoRegisterForEvents, IResultListener
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
            // AllListeners(x => x.StoryStarting(story));
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
       
        public void Handle(StoryStartingEvent eventToHandle)
        {
            AllListeners(x => x.Handle(eventToHandle));
        }

       
    }
}