using System;
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

    public class GatheringResultListener : IEventHandler<ScenarioStarting>, 
                                            IEventHandler<SessionFinished>,
                                            IEventHandler<ScenarioFinished>, 
                                            IEventHandler<LineExecuted>,                                           
                                           IEventHandler<StoryStarting>
    {
        protected readonly IGatheredResultHandler Handler;
        private readonly GatheredResultSet Result = new GatheredResultSet();

        protected GatheringResultListener(IGatheredResultHandler handler)
        {
            Handler = handler;
        }


        public void Handle(ScenarioStarting eventToHandle)
        {
            IScenario scenario = eventToHandle.Scenario;
            CurrentStory().AddScenario(new ScenarioResult {Name = scenario.Name, Id = scenario.Id});
        }

        public void Handle(SessionFinished eventToHandle)
        {
            Handler.Handle(Result);
        }

        private StoryResult CurrentStory()
        {
            return Result.Stories.Last();
        }


        private ScenarioResult CurrentScenario()
        {
            return CurrentStory().Scenarios.Last();
        }


        public void Handle(StoryStarting eventToHandle)
        {
            Story story = eventToHandle.Story;
            var storyResult = new StoryResult
                                  {
                                      Id = story.Id,
                                      Summary = story.Summary
                                  };
            Result.Add(storyResult);
        }

        public void Handle(LineExecuted eventToHandle)
        {
            if (eventToHandle.Status == ExecutionStatus.Failed)
            {
                if (!string.IsNullOrEmpty(eventToHandle.SuccessPart))
                    CurrentScenario().AddLine(ExecutionStatus.Passed, eventToHandle.SuccessPart);
                CurrentScenario().AddLine(ExecutionStatus.Failed, eventToHandle.FailedPart);
                CurrentScenario().FailureMessage = eventToHandle.Message;
            }
            else if (eventToHandle.Status == ExecutionStatus.Passed)
            {
                CurrentScenario().AddLine(ExecutionStatus.Passed, eventToHandle.Line);
            }
            else
            {
                CurrentScenario().AddLine(ExecutionStatus.Pending, eventToHandle.Line);
                CurrentScenario().CouldNotInterpret = true;
                CurrentScenario().Suggestion = eventToHandle.Suggestion ?? "";
            }
        }

        public void Handle(ScenarioFinished eventToHandle)
        {
            CurrentScenario().Status = eventToHandle.Status;
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
            get { return ScenariosByStatus(ExecutionStatus.Pending); }
        }

        public IList<ScenarioResult> FailedScenarios
        {
            get { return ScenariosByStatus(ExecutionStatus.Failed); }
        }

        public IList<ScenarioResult> PassedScenarios
        {
            get { return ScenariosByStatus(ExecutionStatus.Passed); }
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

        public List<ScenarioResult> ScenariosByStatus(ExecutionStatus status)
        {
            return Scenarios.Where(s => s.Status == status).ToList();
        }

        public bool HasAnyStories()
        {
            return _stories.Any();
        }

        public bool HasAnyScenarios(ExecutionStatus executionStatus)
        {
            return _stories.Any(s => s.HasAnyScenarios(executionStatus));
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
            get { return ScenariosByStatus(ExecutionStatus.Pending); }
        }

        public IList<ScenarioResult> FailedScenarios
        {
            get { return ScenariosByStatus(ExecutionStatus.Failed); }
        }

        public IList<ScenarioResult> PassedScenarios
        {
            get { return ScenariosByStatus(ExecutionStatus.Passed); }
        }

        public void AddScenario(ScenarioResult scenarioResult)
        {
            _scenarios.Add(scenarioResult);
        }

        public bool HasAnyScenarios(ExecutionStatus executionStatus)
        {
            return _scenarios.Any(s => s.Status == executionStatus);
        }

        public List<ScenarioResult> ScenariosByStatus(ExecutionStatus status)
        {
            return Scenarios.Where(s => s.Status == status).ToList();
        }
    }

    public class ScenarioResult
    {
        private readonly List<ScenarioLineResult> _lines = new List<ScenarioLineResult>();
        public ExecutionStatus Status { get; set; }

        public IEnumerable<ScenarioLineResult> Lines
        {
            get { return _lines; }
        }

        public string FailureMessage { get; set; }

        public string Name { get; set; }

        public string Id { get; set; }

        public string Suggestion { get; set; }

        public bool CouldNotInterpret { get; set; }

        public void AddLine(ExecutionStatus status, string text)
        {
            _lines.Add(new ScenarioLineResult {Status = status, Text = text});
        }
    }

    public class ScenarioLineResult
    {
        public ExecutionStatus Status { get; set; }
        public string Text { get; set; }
    }


}