using System;
using System.Collections.Generic;
using System.Linq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public abstract class InPlaceStoryRunnerBase : IStoryHandler
    {
        protected readonly IResultListener ResultListener;
        private readonly IStoryFilter _filter;
        private readonly ISessionContext _context;
        private readonly IEventBus _eventBus;
        private readonly IScenarioPreprocessor _preprocessor;

        protected InPlaceStoryRunnerBase(IResultListener resultListener, IScenarioPreprocessor preprocessor, IStoryFilter filter, ISessionContext context, IEventBus eventBus)
        {
            ResultListener = resultListener;
            _preprocessor = preprocessor;
            _filter = filter;
            _context = context;
            _eventBus = eventBus;

            Result = new JobResult();
        }

        public void HandleStory(Story story)
        {
           _eventBus.Raise(new StoryStartingEvent {Story = story});
            
            Scenario[] scenariosMatchingFilter = GetScenariosMatchingFilter(story);

            using (StoryContext contextForStory = _context.GetContextForStory())
            {
                Execute(story, scenariosMatchingFilter, contextForStory);
            }
        }

        protected abstract void Execute(Story story, IEnumerable<Scenario> scenariosMatchingFilter, StoryContext context);

        private Scenario[] GetScenariosMatchingFilter(Story story)
        {
            return GetScenarios(story).Where(s => _filter.Include(story, s)).ToArray();
        }

        private IEnumerable<Scenario> GetScenarios(Story story)
        {
            return story.Scenarios.SelectMany(scenario => _preprocessor.Preprocess(scenario)).ToArray();
        }

        public void Finished()
        {
            _eventBus.Raise(new SessionFinishedEvent());
        }
        public JobResult GetResult()
        {
            return Result;
        }

        protected JobResult Result { get; set; }
    }
}