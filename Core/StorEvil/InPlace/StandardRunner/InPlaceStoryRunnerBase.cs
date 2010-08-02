using System;
using System.Collections.Generic;
using System.Linq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public abstract class InPlaceStoryRunnerBase : IStoryHandler
    {
        protected readonly IResultListener ResultListener;
        private readonly IStoryFilter _filter;
        private readonly ISessionContext _context;
        protected readonly IEventBus EventBus;
        private readonly IScenarioPreprocessor _preprocessor;

        protected InPlaceStoryRunnerBase(IResultListener resultListener, IScenarioPreprocessor preprocessor, IStoryFilter filter, ISessionContext context, IEventBus eventBus)
        {
            ResultListener = resultListener;
            _preprocessor = preprocessor;
            _filter = filter;
            _context = context;
            EventBus = eventBus;

            Result = new JobResult();
        }

        public void HandleStory(Story story)
        {
           EventBus.Raise(new StoryStartingEvent {Story = story});
            
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
            EventBus.Raise(new SessionFinishedEvent());
        }
        public JobResult GetResult()
        {
            return Result;
        }

        protected JobResult Result { get; set; }
    }
}