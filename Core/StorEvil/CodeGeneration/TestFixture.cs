using StorEvil.Context;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Interpreter;

namespace StorEvil.CodeGeneration
{
    public class TestFixture
    {
        private StoryContext _storyContext;
        private ScenarioContext _scenarioContext;
        private StandardScenarioInterpreter _interpreter;
        private ScenarioLineExecuter _scenarioLineExecuter;
        private IResultListener _listener;
       
        private Scenario _currentScenario;
        private object _debugContexts;

        protected void BeforeAll()
        {
            _storyContext = TestSession.SessionContext(GetType().Assembly.Location).GetContextForStory();
            _interpreter = new StandardScenarioInterpreter();
            _scenarioLineExecuter = new ScenarioLineExecuter(_interpreter, _listener);
        }

        protected void SetListener(IResultListener listener)
        {
            _listener = listener;
        }

        protected void BeforeEach()
        {
            _scenarioContext = _storyContext.GetScenarioContext();
        }

        protected void ExecuteLine(string line)
        {
            _debugContexts = null;
            _scenarioLineExecuter.ExecuteLine(_currentScenario, _scenarioContext, line);          
        }

        protected void SetCurrentScenario(string id, string summary)
        {   
            _currentScenario = new Scenario(id, summary, new ScenarioLine[0]);
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
            _storyContext.Dispose();

            // hack for now
            TestSession.EndSession();
        }
    }
}