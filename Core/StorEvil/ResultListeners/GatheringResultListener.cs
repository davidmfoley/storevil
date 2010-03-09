using System.Collections.Generic;
using System.Linq;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.ResultListeners
{
    public interface IGatheredResultHandler
    {
        void Handle(GatheredResultSet result);
    }

    public class GatheringResultListener : IResultListener
    {
        protected GatheringResultListener(IGatheredResultHandler handler)
        {
            Handler = handler;
        }

        private readonly GatheredResultSet Result = new GatheredResultSet();
        protected readonly IGatheredResultHandler Handler;

        public void StoryStarting(Story story)
        {
            var storyResult = new StoryResult
                                  {
                                      Id = story.Id,
                                      Summary = story.Summary
                                  };
            Result.Add(storyResult);
        }

        public void ScenarioStarting(Scenario scenario)
        {
            CurrentStory().AddScenario(new ScenarioResult {Name = scenario.Name, Id = scenario.Id});
        }

        private StoryResult CurrentStory()
        {
            return Result.Stories.Last();
        }

        public void ScenarioFailed(Scenario scenario, string successPart, string failedPart, string message)
        {
            CurrentScenario().FailureMessage = message;
            CurrentScenario().Status = ScenarioStatus.Failed;
            if (!string.IsNullOrEmpty(successPart))
                CurrentScenario().AddLine(ScenarioStatus.Passed, successPart);
            CurrentScenario().AddLine(ScenarioStatus.Failed, failedPart);
        }

        public void CouldNotInterpret(Scenario scenario, string line)
        {
            CurrentScenario().Status = ScenarioStatus.Pending;
            CurrentScenario().AddLine(ScenarioStatus.Pending, line);
        }

        public void Success(Scenario scenario, string line)
        {
            CurrentScenario().AddLine(ScenarioStatus.Passed, line);
        }

        private ScenarioResult CurrentScenario()
        {
            return CurrentStory().Scenarios.Last();
        }

        public void ScenarioSucceeded(Scenario scenario)
        {
        }

        public void Finished()
        {
            Handler.Handle(Result);
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

        public void Add(StoryResult storyResult)
        {
            _stories.Add(storyResult);
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

        public void AddScenario(ScenarioResult scenarioResult)
        {
            _scenarios.Add(scenarioResult);
        }

        public bool HasAnyScenarios(ScenarioStatus scenarioStatus)
        {
            return _scenarios.Any(s => s.Status == scenarioStatus);
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
        public List<ScenarioResult> ScenariosByStatus(ScenarioStatus status)
        {
            return Scenarios.Where(s => s.Status == status).ToList();
        }
    }

    public class ScenarioResult
    {
        public ScenarioStatus Status { get; set; }
        private readonly List<ScenarioLineResult> _lines = new List<ScenarioLineResult>();

        public IEnumerable<ScenarioLineResult> Lines
        {
            get { return _lines; }
        }

        public string FailureMessage { get; set; }

        public string Name { get; set; }

        public string Id { get; set; }

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