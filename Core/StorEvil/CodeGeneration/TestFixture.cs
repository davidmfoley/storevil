using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.InPlace;
using StorEvil.Interpreter;

namespace StorEvil.CodeGeneration
{
    public class TestFixture
    {
        private StoryContext _sessionContext;
        private ScenarioContext _scenarioContext;
        private StandardScenarioInterpreter _interpreter;
        private ScenarioLineExecuter _scenarioLineExecuter;
       
        private Scenario _currentScenario;
        private object _debugContexts;
        private EventBus _eventBus;
        private LineStatus _lastStatus;

        protected void BeforeAll()
        {
            _eventBus = StorEvilEvents.Bus;

            _sessionContext = TestSession.SessionContext(GetType().Assembly.Location).GetContextForStory();
            _interpreter = TestSession.GetInterpreter();
          
            _scenarioLineExecuter = new ScenarioLineExecuter(_interpreter, _eventBus);
        }

        protected void SetListener(object listener)
        {            
            StorEvilEvents.Bus.Register(listener);
           // _listener = listener;
        }

        protected void BeforeEach()
        {
            _scenarioContext = _sessionContext.GetScenarioContext();
            _lastStatus = LineStatus.Passed;
        }

        protected void ExecuteLine(string line)
        {
            if (_lastStatus != LineStatus.Passed)
                return;

            _debugContexts = null;

            
            _lastStatus = _scenarioLineExecuter.ExecuteLine(_currentScenario, _scenarioContext, line);          
        }

        protected void SetCurrentScenario(string id, string summary)
        {   
            _currentScenario = new Scenario("", id, summary, new ScenarioLine[0]);
        }

        protected void AfterEach()
        {
            _scenarioContext.Dispose();
        }

        protected object GetContexts()
        {
            return _debugContexts ?? (_debugContexts = new ContextViewer().Create(_scenarioContext.Contexts));
        }
        protected void AfterAll()
        {
            _sessionContext.Dispose();

            // hack for now
            TestSession.EndSession();
        }
    }
}