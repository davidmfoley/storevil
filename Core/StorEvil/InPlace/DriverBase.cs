using System;
using System.Linq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public abstract class DriverBase : MarshalByRefObject, IStoryHandler
    {
        protected JobResult Result = new JobResult();
        private IResultListener _listener;

        private ScenarioLineExecuter LineExecuter;
        private StoryContextFactory ContextFactory;
        private ScenarioContext CurrentScenarioContext;
        private Scenario CurrentScenario;
        

        protected DriverBase(IResultListener listener)
        {
            _listener = listener;
            
            ScenarioInterpreter = new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler()));           
            LineExecuter = new ScenarioLineExecuter(new MemberInvoker(), ScenarioInterpreter, _listener);
            ContextFactory = new StoryContextFactory();            
        } 

        protected void AddAssembly(string location)
        {
            ContextFactory.AddAssembly(location);
        }

        protected object[] GetContexts()
        {
            return CurrentScenarioContext.Contexts.Values.ToArray();
        }
        protected Scenario[] GetScenarios(Story story)
        {
           
            return story.Scenarios.SelectMany(s=> new ScenarioPreprocessor().Preprocess(s)).ToArray();
        }

        protected ScenarioInterpreter ScenarioInterpreter;

        public abstract void HandleStory(Story story);

        public void Finished()
        {
            
        }

        protected void ExecuteLine(string line)
        {
            LastStatus = LineExecuter.ExecuteLine(CurrentScenario, CurrentScenarioContext, line);            
        }
        public JobResult GetResult()
        {
            return Result;
        }

        protected IDisposable StartScenario(Story story, Scenario scenario)
        {
            _listener.ScenarioStarting(scenario);

            CurrentScenarioContext = ContextFactory.GetContextForStory(story).GetScenarioContext();
            CurrentScenario = scenario;
            LastStatus = LineStatus.Passed;
            ScenarioInterpreter.NewScenario();

            return CurrentScenarioContext;
        }

        protected LineStatus LastStatus
        {
            get; set;
        }

        protected bool ShouldContinue
        {
            get
            {
                return LastStatus == LineStatus.Passed;
            }
        }

        protected void CollectScenarioResult()
        {
            if (LastStatus == LineStatus.Failed)
            {
                Result.Failed++;
            } 
            else if (LastStatus == LineStatus.Pending)
            {
                Result.Pending++;
            } 
            else
            {
                Result.Succeeded++; 
                _listener.ScenarioSucceeded(CurrentScenario);
            }
        }
    }
}