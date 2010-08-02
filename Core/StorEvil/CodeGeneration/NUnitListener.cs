using System.Diagnostics;
using NUnit.Framework;
using StorEvil.Events;

namespace StorEvil.CodeGeneration
{
    public class NUnitListener : IEventHandler<LineExecutedEvent>
    {                  
        public void Handle(LineExecutedEvent eventToHandle)
        {
            if (eventToHandle.Status == ExecutionStatus.Failed)
            {
                Debug.Write(eventToHandle.SuccessPart);
                Debug.WriteLine("{ " + eventToHandle.FailedPart + " -- FAILED }");
                Assert.Fail(eventToHandle.Message);
            }
            else if (eventToHandle.Status == ExecutionStatus.Passed)
            {
                Debug.WriteLine(eventToHandle.Line);
            }
            else
            {
                var message = "Could not interpret:\r\n" + eventToHandle.Line +
                         "\r\nSuggestion:\r\n" + eventToHandle.Suggestion;
                Debug.WriteLine(message);
            }
        }
    }
}