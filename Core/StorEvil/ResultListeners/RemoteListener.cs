using System;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.InPlace;

namespace StorEvil.ResultListeners
{
    public class RemoteListener : MarshalByRefObject, IResultListener, IEventHandler<SessionFinishedEvent>
    {
       

        private IResultListener _inner;

        public void Handle(StoryStartingEvent e)
        {
           // _inner.Handle(e);
            //_inner.StoryStarting(story);
        }

        public void Handle(SessionFinishedEvent eventToHandle)
        {
            //_inner.Handle(eventToHandle);
        }

        public void ScenarioStarting(Scenario scenario)
        {
           // _inner.ScenarioStarting(scenario);
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


        public RemoteListener(IResultListener inner)
        {
            _inner = inner;
            StorEvilEvents.Bus.Register(this);
        } 
    }
}