using StorEvil.Core;
using StorEvil.ResultListeners;

namespace StorEvil.InPlace
{
    public interface IResultListener
    {
        void StoryStarting(Story story);
        void ScenarioStarting(Scenario scenario);
        void ScenarioFailed(ScenarioFailureInfo scenarioFailureInfo);
        void CouldNotInterpret(CouldNotInterpretInfo couldNotInterpretInfo);
        void Success(Scenario scenario, string line);
        void ScenarioSucceeded(Scenario scenario);
        void Finished();
    }

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

    public class CouldNotInterpretInfo
    {
        private Scenario _scenario;
        private string _line;

        public CouldNotInterpretInfo(Scenario scenario, string line)
        {
            _scenario = scenario;
            _line = line;
        }

        public Scenario Scenario
        {
            get { return _scenario; }
        }

        public string Line
        {
            get { return _line; }
        }
    }
}