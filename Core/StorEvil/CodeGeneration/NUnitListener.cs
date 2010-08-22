using System.Diagnostics;
using StorEvil.Events;

namespace StorEvil.CodeGeneration
{
    public class NUnitListener : IHandle<LineExecuted>
    {                  
        private NUnitAssertWrapper _assertWrapper = new NUnitAssertWrapper();
        public void Handle(LineExecuted eventToHandle)
        {
            if (eventToHandle.Status == ExecutionStatus.Failed)
            {
                Debug.Write(eventToHandle.SuccessPart);
                Debug.WriteLine("{ " + eventToHandle.FailedPart + " -- FAILED }");

                _assertWrapper.Fail(eventToHandle.ExceptionInfo);
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