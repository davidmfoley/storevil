using System;
using System.Diagnostics;
using StorEvil.Events;

namespace StorEvil.CodeGeneration
{
    public class NUnitListener : IHandle<LineFailed>, IHandle<LinePassed>, IHandle<LinePending> 
    {                  
        private NUnitAssertWrapper _assertWrapper = new NUnitAssertWrapper();
       

        public void Handle(LineFailed eventToHandle)
        {
            Debug.Write(eventToHandle.SuccessPart);
            Debug.WriteLine("{ " + eventToHandle.FailedPart + " -- FAILED }");

            _assertWrapper.Fail(eventToHandle.ExceptionInfo);
        }

        public void Handle(LinePassed eventToHandle)
        {
            Debug.WriteLine(eventToHandle.Line);
        }

        public void Handle(LinePending eventToHandle)
        {
            var message = "Could not interpret:\r\n" + eventToHandle.Line +
                        "\r\nSuggestion:\r\n" + eventToHandle.Suggestion;
            Debug.WriteLine(message);
        }
    }
}