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
        private readonly IScenarioPreprocessor _preprocessor;

        protected InPlaceStoryRunnerBase(IResultListener resultListener,
                                         IScenarioPreprocessor preprocessor,
                                         IStoryFilter filter, ISessionContext context)
        {
            ResultListener = resultListener;
            _preprocessor = preprocessor;
            _filter = filter;
            _context = context;

            Result = new JobResult();
        }

        public void HandleStory(Story story)
        {
            ResultListener.StoryStarting(story);
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
            ResultListener.Finished();
        }
        public JobResult GetResult()
        {
            return Result;
        }

        protected JobResult Result { get; set; }
    }
}