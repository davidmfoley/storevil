using System.Collections.Generic;
using System.Linq;
using StorEvil.Core;
using StorEvil.Events;

namespace StorEvil.ResultListeners
{
    public interface IGatheredResultHandler
    {
        void Handle(GatheredResultSet result);
    }

    public class GatheringResultListener : AutoRegisterForEvents,
                                           IEventHandler<ScenarioStartingEvent, SessionFinishedEvent, ScenarioFailedEvent>,
                                           IEventHandler<ScenarioPendingEvent, LineInterpretedEvent, LineNotInterpretedEvent>
    {
        protected readonly IGatheredResultHandler Handler;
        private readonly GatheredResultSet Result = new GatheredResultSet();

        protected GatheringResultListener(IGatheredResultHandler handler)
        {
            Handler = handler;
        }

        public void Handle(ScenarioPendingEvent eventToHandle)
        {
            CurrentScenario().Status = ScenarioStatus.Pending;
        }

        public void Handle(LineNotInterpretedEvent eventToHandle)
        {
            CurrentScenario().AddLine(ScenarioStatus.Pending, eventToHandle.Line);
            CurrentScenario().CouldNotInterpret = true;
            CurrentScenario().Suggestion = eventToHandle.Suggestion ?? "";
        }

        public void Handle(LineInterpretedEvent eventToHandle)
        {
            CurrentScenario().AddLine(ScenarioStatus.Passed, eventToHandle.Line);
        }

        public void Handle(ScenarioStartingEvent eventToHandle)
        {
            IScenario scenario = eventToHandle.Scenario;
            CurrentStory().AddScenario(new ScenarioResult {Name = scenario.Name, Id = scenario.Id});
        }

        public void Handle(SessionFinishedEvent eventToHandle)
        {
            Handler.Handle(Result);
        }

        public void Handle(ScenarioFailedEvent eventToHandle)
        {
            CurrentScenario().FailureMessage = eventToHandle.Message;
            CurrentScenario().Status = ScenarioStatus.Failed;
            if (!string.IsNullOrEmpty(eventToHandle.SuccessPart))
                CurrentScenario().AddLine(ScenarioStatus.Passed, eventToHandle.SuccessPart);
            CurrentScenario().AddLine(ScenarioStatus.Failed, eventToHandle.FailedPart);
        }

        private StoryResult CurrentStory()
        {
            return Result.Stories.Last();
        }


        private ScenarioResult CurrentScenario()
        {
            return CurrentStory().Scenarios.Last();
        }


        public void Handle(StoryStartingEvent eventToHandle)
        {
            Story story = eventToHandle.Story;
            var storyResult = new StoryResult
                                  {
                                      Id = story.Id,
                                      Summary = story.Summary
                                  };
            Result.Add(storyResult);
        }
    }

    public class GatheredResultSet
    {
        private readonly List<StoryResult> _stories = new List<StoryResult>();

        public IEnumerable<StoryResult> Stories
        {
            get { return _stories; }
        }

        public int StoryCount
        {
            get { return Stories.Count(); }
        }

        public IList<ScenarioResult> Scenarios
        {
            get { return _stories.SelectMany(x => x.Scenarios).ToList(); }
        }

        public IList<ScenarioResult> PendingScenarios
        {
            get { return ScenariosByStatus(ScenarioStatus.Pending); }
        }

        public IList<ScenarioResult> FailedScenarios
        {
            get { return ScenariosByStatus(ScenarioStatus.Failed); }
        }

        public IList<ScenarioResult> PassedScenarios
        {
            get { return ScenariosByStatus(ScenarioStatus.Passed); }
        }

        public IList<string> Suggestions
        {
            get
            {
                return Scenarios
                    .Select(s => s.Suggestion)
                    .Where(s => s != null)
                    .Distinct()
                    .ToList();
            }
        }

        public void Add(StoryResult storyResult)
        {
            _stories.Add(storyResult);
        }

        public List<ScenarioResult> ScenariosByStatus(ScenarioStatus status)
        {
            return Scenarios.Where(s => s.Status == status).ToList();
        }

        public bool HasAnyStories()
        {
            return _stories.Any();
        }

        public bool HasAnyScenarios(ScenarioStatus scenarioStatus)
        {
            return _stories.Any(s => s.HasAnyScenarios(scenarioStatus));
        }
    }

    public class StoryResult
    {
        private readonly List<ScenarioResult> _scenarios = new List<ScenarioResult>();

        public IEnumerable<ScenarioResult> Scenarios
        {
            get { return _scenarios; }
        }

        public string Id { get; set; }
        public string Summary { get; set; }

        public IList<ScenarioResult> PendingScenarios
        {
            get { return ScenariosByStatus(ScenarioStatus.Pending); }
        }

        public IList<ScenarioResult> FailedScenarios
        {
            get { return ScenariosByStatus(ScenarioStatus.Failed); }
        }

        public IList<ScenarioResult> PassedScenarios
        {
            get { return ScenariosByStatus(ScenarioStatus.Passed); }
        }

        public void AddScenario(ScenarioResult scenarioResult)
        {
            _scenarios.Add(scenarioResult);
        }

        public bool HasAnyScenarios(ScenarioStatus scenarioStatus)
        {
            return _scenarios.Any(s => s.Status == scenarioStatus);
        }

        public List<ScenarioResult> ScenariosByStatus(ScenarioStatus status)
        {
            return Scenarios.Where(s => s.Status == status).ToList();
        }
    }

    public class ScenarioResult
    {
        private readonly List<ScenarioLineResult> _lines = new List<ScenarioLineResult>();
        public ScenarioStatus Status { get; set; }

        public IEnumerable<ScenarioLineResult> Lines
        {
            get { return _lines; }
        }

        public string FailureMessage { get; set; }

        public string Name { get; set; }

        public string Id { get; set; }

        public string Suggestion { get; set; }

        public bool CouldNotInterpret { get; set; }

        public void AddLine(ScenarioStatus status, string text)
        {
            _lines.Add(new ScenarioLineResult {Status = status, Text = text});
        }
    }

    public class ScenarioLineResult
    {
        public ScenarioStatus Status { get; set; }
        public string Text { get; set; }
    }

    public enum ScenarioStatus
    {
        Passed,
        Failed,
        Pending
    }
}