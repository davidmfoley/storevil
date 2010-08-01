using System;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    public interface IResultListener : IEventHandler<StoryStartingEvent>
    {               
        void ScenarioStarting(Scenario scenario);

        void ScenarioFailed(ScenarioFailureInfo scenarioFailureInfo);
        void ScenarioPending(ScenarioPendingInfo scenarioPendingInfo);
        void Success(Scenario scenario, string line);

        void ScenarioSucceeded(Scenario scenario);

        //void Finished();
    }

    [Serializable]
    public class ScenarioFailureInfo
    {
        private Scenario _scenario;
        private string _successPart;
        private string _failedPart;
        private string _message;

        public ScenarioFailureInfo(Scenario scenario, string successPart, string failedPart, string message)
        {
            _scenario = scenario;
            _successPart = successPart;
            _failedPart = failedPart;
            _message = message;
        }

        public Scenario Scenario
        {
            get { return _scenario; }
        }

        public string SuccessPart
        {
            get { return _successPart; }
        }

        public string FailedPart
        {
            get { return _failedPart; }
        }

        public string Message
        {
            get { return _message; }
        }
    }

    [Serializable]
    public class ScenarioPendingInfo
    {
        public ScenarioPendingInfo(Scenario scenario, string line)
        {
            Scenario = scenario;
            Line = line;
        }

        public ScenarioPendingInfo(Scenario scenario, string line, string suggestion) : this(scenario, line)
        {
            Suggestion = suggestion;
            CouldNotInterpret = true;
        }

        public bool CouldNotInterpret { get; set; }

        public Scenario Scenario { get;  set; }
        public string Suggestion { get; set; }

        public string Line { get;  set; }
    }
}