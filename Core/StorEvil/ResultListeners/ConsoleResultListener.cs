using System;
using System.Diagnostics;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.InPlace;

namespace StorEvil.ResultListeners
{
    public abstract class WriterListener :
        IHandle<ScenarioStarting>, IHandle<SessionFinished>, IHandle<ScenarioFinished>,
        IHandle<LineExecuted>,
        IHandle<StoryStarting>
    {
        protected abstract void DoWrite(ConsoleColor white, params string[] s);
     
       

        public void Handle(StoryStarting eventToHandle)
        {
            var story = eventToHandle.Story;
            DoWrite(ConsoleColor.White, "\r\n" + "\r\nSTORY: " + story.Id + "\r\n" + story.Summary);
        }

      

        public void Handle(ScenarioStarting eventToHandle)
        {
            DoWrite(ConsoleColor.White, "\r\nSCENARIO: " + eventToHandle.Scenario.Name);
        }

        public void Handle(SessionFinished eventToHandle)
        {
            DoWrite(ConsoleColor.Green, "Finished");
        }             

        public void Handle(ScenarioFinished eventToHandle)
        {
            if (eventToHandle.Status == ExecutionStatus.Passed)
                DoWrite(ConsoleColor.Green, "Scenario succeeded");
        }

        public void Handle(LineExecuted eventToHandle)
        {
            if (eventToHandle.Status == ExecutionStatus.Pending)
            {
                DoWrite(ConsoleColor.Yellow, eventToHandle.Line + " -- Pending");
            }
            else if (eventToHandle.Status == ExecutionStatus.CouldNotInterpret) 
            {
                DoWrite(ConsoleColor.Yellow, eventToHandle.Line + " -- Could not interpret");
            }
            else if (eventToHandle.Status == ExecutionStatus.Passed)
            {
                DoWrite(ConsoleColor.Green, eventToHandle.Line);
            }
            else
            {
                DoWrite(ConsoleColor.Yellow, eventToHandle.SuccessPart);
                DoWrite(ConsoleColor.Red, " " + eventToHandle.FailedPart + "\r\nFAILED\r\n",
                        eventToHandle.ExceptionInfo + "\r\n");
                
            }
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