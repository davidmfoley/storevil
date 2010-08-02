using System;
using System.Diagnostics;
using NUnit.Framework;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.InPlace;

namespace StorEvil.CodeGeneration
{
    public class NUnitListener : 
        IEventHandler<ScenarioFailedEvent, ScenarioPendingEvent, LineInterpretedEvent>,
        IEventHandler<LineNotInterpretedEvent>
    {           
       
        public void Handle(ScenarioFailedEvent eventToHandle)
        {
            Debug.Write(eventToHandle.SuccessPart);
            Debug.WriteLine("{ " + eventToHandle.FailedPart + " -- FAILED }");
            Assert.Fail(eventToHandle.Message);
        }

        public void Handle(ScenarioPendingEvent eventToHandle)
        {
            var message = "Pending :\r\n" + eventToHandle.Line ;
            Debug.WriteLine(message);
            Assert.Ignore(message);
        }

        public void Handle(LineInterpretedEvent eventToHandle)
        {
            Debug.WriteLine(eventToHandle.Line);
        }

        public void Handle(LineNotInterpretedEvent eventToHandle)
        {
            var message = "Could not interpret:\r\n" + eventToHandle.Line +
                          "\r\nSuggestion:\r\n" + eventToHandle.Suggestion;
            Debug.WriteLine(message);
            Assert.Ignore(message);
        }
    }
}