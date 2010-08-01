using System;
using System.Diagnostics;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.ResultListeners
{
    public abstract class WriterListener : AutoRegisterForEvents, IResultListener, IEventHandler<SessionFinishedEvent>
    {
        protected abstract void DoWrite(ConsoleColor white, params string[] s);

        public void StoryStarting(Story story)
        {
            DoWrite(ConsoleColor.White, "\r\n" + "\r\nSTORY: " + story.Id + "\r\n" + story.Summary);
        }

        public void ScenarioStarting(Scenario scenario)
        {
            DoWrite(ConsoleColor.White, "\r\nSCENARIO: " + scenario.Name);
        }

        public void ScenarioFailed(ScenarioFailureInfo scenarioFailureInfo)
        {
            DoWrite(ConsoleColor.Yellow, scenarioFailureInfo.SuccessPart);
            DoWrite(ConsoleColor.Red, " " + scenarioFailureInfo.FailedPart + "\r\nFAILED\r\n",
                    scenarioFailureInfo.Message + "\r\n");
        }

        public void ScenarioPending(ScenarioPendingInfo scenarioPendingInfo)
        {
            var message = scenarioPendingInfo.CouldNotInterpret ? "Could not interpret" : "Pending";
            DoWrite(ConsoleColor.Yellow, scenarioPendingInfo.Line + " -- " + message);

            if (scenarioPendingInfo.CouldNotInterpret && scenarioPendingInfo.Suggestion != null)
                DoWrite(ConsoleColor.Gray, scenarioPendingInfo.Suggestion);
        }

        public void Success(Scenario scenario, string line)
        {
            DoWrite(ConsoleColor.Green, line);
        }

        public void ScenarioSucceeded(Scenario scenario)
        {
            DoWrite(ConsoleColor.Green, "Scenario succeeded");
        }   

       

        public void Handle(StoryStartingEvent eventToHandle)
        {
            var story = eventToHandle.Story;
            DoWrite(ConsoleColor.White, "\r\n" + "\r\nSTORY: " + story.Id + "\r\n" + story.Summary);
        }

        public void Handle(SessionFinishedEvent eventToHandle)
        {
            DoWrite(ConsoleColor.Green, "Finished");
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