using System;
using System.Diagnostics;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.InPlace;

namespace StorEvil.ResultListeners
{
    public abstract class WriterListener :
        IHandle<SessionFinished>,
        IHandle<ScenarioStarting>, IHandle<ScenarioFailed>, IHandle<ScenarioPassed>, IHandle<ScenarioPending>,
        IHandle<LineFailed>, IHandle<LinePassed>, IHandle<LinePending>,
        IHandle<StoryStarting>, IHandle<GenericInformation>
    {
        private int _passingScenarios;
        private int _failedScenarios;
        private int _pendingScenarios;

        protected abstract void DoWrite(ConsoleColor white, params string[] s);
     
        public void Handle(StoryStarting eventToHandle)
        {
            var story = eventToHandle.Story;
            DoWrite(ConsoleColor.White, "\r\n" + "\r\nSTORY: " + story.Id + "\r\n" + story.Summary);
        } 

        public void Handle(SessionFinished eventToHandle)
        {
            var color = GetSummaryColor();
            var summary = string.Format(Environment.NewLine + "Finished - {0} passed, {1} failed, {2} pending", _passingScenarios, _failedScenarios, _pendingScenarios);

            DoWrite(color, summary);
        }

        private ConsoleColor GetSummaryColor()
        {
            if (_failedScenarios > 0)
            {
                return ConsoleColor.Red;
            }
            else if (_pendingScenarios > 0)
            {
                return ConsoleColor.Yellow;
            }
            else
            {
                return ConsoleColor.Green;
            }
        }

        public void Handle(ScenarioStarting eventToHandle)
        {
            DoWrite(ConsoleColor.White, "\r\nSCENARIO: " + eventToHandle.Scenario.Name);
        }

        public void Handle(ScenarioPassed eventToHandle)
        {
            _passingScenarios++;

            DoWrite(ConsoleColor.Green, "Scenario succeeded");
        }

        public void Handle(ScenarioFailed eventToHandle)
        {
            _failedScenarios++;

            DoWrite(ConsoleColor.Red, "Scenario failed");
            if (!string.IsNullOrEmpty(eventToHandle.ExceptionInfo))
            {
                DoWrite(ConsoleColor.Red, Environment.NewLine + eventToHandle.ExceptionInfo);
            }
        }

        public void Handle(ScenarioPending eventToHandle)
        {
            _pendingScenarios++;

            DoWrite(ConsoleColor.Yellow, "Scenario pending");
        }

        public void Handle(LineFailed eventToHandle)
        {
            DoWrite(ConsoleColor.Yellow, eventToHandle.SuccessPart);
            DoWrite(ConsoleColor.Red, " " + eventToHandle.FailedPart + "\r\nFAILED\r\n",
                    eventToHandle.ExceptionInfo + "\r\n");
        }

        public void Handle(LinePassed eventToHandle)
        {
            DoWrite(ConsoleColor.Green, eventToHandle.Line);
        }

        public void Handle(LinePending eventToHandle)
        {
            DoWrite(ConsoleColor.Yellow, eventToHandle.Line + " -- Pending");
        }

        public void Handle(GenericInformation eventToHandle)
        {
            DoWrite(ConsoleColor.White, eventToHandle.Text);
        }
    }

    public class ConsoleResultListener : WriterListener
    {
        protected override void DoWrite(ConsoleColor color, params string[] lines)
        {
            if (ColorEnabled)
                System.Console.ForegroundColor = color;

            foreach (var s in lines)
                System.Console.WriteLine(s);

            if (ColorEnabled)
                System.Console.ResetColor();
        }

        public bool ColorEnabled { get; set; }
    }

    public class DebugListener : WriterListener
    {
        protected override void DoWrite(ConsoleColor white, params string[] s)
        {
            foreach (var message in s)
            {
                Debug.WriteLine(message);
            }
        }
    }
}