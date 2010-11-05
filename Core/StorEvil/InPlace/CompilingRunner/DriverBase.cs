using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Interpreter;
using StorEvil.Interpreter.ParameterConverters;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    [DebuggerStepThrough]
    public abstract class DriverBase : MarshalByRefObject, IStoryHandler
    {
        protected JobResult Result = new JobResult();
        private readonly ScenarioLineExecuter LineExecuter;
        private readonly SessionContext _context;
        private IEventBus _eventBus;

        protected StoryContext CurrentStoryContext;
        private ScenarioContext CurrentScenarioContext;
        private Scenario CurrentScenario;

        private string ExceptionInfo;
        protected LineStatus LastStatus { get; set; }
        
        protected DriverBase(IEventBus eventBus)
        {
            //ResultListener = resultListener;
            _eventBus = eventBus;
            var assemblyRegistry = new AssemblyRegistry(GetAssemblies());
            ScenarioInterpreter = new ScenarioInterpreter(new InterpreterForTypeFactory(assemblyRegistry), new MostRecentlyUsedContext(), new DefaultLanguageService());           
            LineExecuter = new ScenarioLineExecuter(ScenarioInterpreter, _eventBus);
            _context = new SessionContext(assemblyRegistry);
            ParameterConverter.AddCustomConverters(assemblyRegistry);
        }

        protected abstract IEnumerable<string> GetAssemblies();

        
        protected object[] GetContexts()
        {
            return CurrentScenarioContext.Contexts.Values.ToArray();
        }

        
        protected Scenario[] GetScenarios(Story story)
        {           
            return story.Scenarios.SelectMany(s=>s.Preprocess()).ToArray();
        }

        protected ScenarioInterpreter ScenarioInterpreter;

        public abstract void HandleStory(Story story);

        
        public JobResult HandleStories(IEnumerable<Story> stories)
        {
            foreach (var story in stories)
            {
                _eventBus.Raise(new StoryStarting { Story = story });
                HandleStory(story);
                _eventBus.Raise(new StoryFinished { Story = story });
            }

            Finished();

            return Result;
        }

        
        public void Finished()
        {          
            CurrentStoryContext.Dispose();
        }

        
        protected object[] ExecuteLine(string line)
        {
            if (LastStatus != LineStatus.Passed)
                return GetContexts();

            LastStatus = LineExecuter.ExecuteLine(CurrentScenario, CurrentScenarioContext, line);

            return GetContexts();
        }
        
        public JobResult GetResult()
        {
            return Result;  
        }
       
        protected void StartScenario(Story story, Scenario scenario)
        {
            
            _eventBus.Raise(new ScenarioStarting {Scenario = scenario});
            if (CurrentStoryContext == null)
                CurrentStoryContext = _context.GetContextForStory();

            CurrentScenarioContext = CurrentStoryContext.GetScenarioContext();
            CurrentScenario = scenario;
            LastStatus = LineStatus.Passed;
            //ScenarioInterpreter.NewScenario();
        }

        protected bool ShouldContinue
        {
            get
            {
                return LastStatus == LineStatus.Passed;
            }
        }

        protected void TryToDisposeScenarioContext()
        {
            try
            {
                CurrentScenarioContext.Dispose();
            }
            catch (Exception e)
            {
                LastStatus = LineStatus.Failed;
                ExceptionInfo = e.ToString();
            }
        }
        
        protected void CollectScenarioResult()
        {
            if (LastStatus == LineStatus.Failed)
            {
                Result.Failed++;
                _eventBus.Raise(new ScenarioFailed() { Scenario = CurrentScenario, ExceptionInfo = ExceptionInfo});     
            } 
            else if (LastStatus == LineStatus.Pending)
            {
                Result.Pending++;
                _eventBus.Raise(new ScenarioPending() { Scenario = CurrentScenario });     
 
            } 
            else
            {
                Result.Succeeded++;
                _eventBus.Raise(new ScenarioPassed { Scenario = CurrentScenario });                
            }
        }
    }
}