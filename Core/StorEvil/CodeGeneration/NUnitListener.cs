using System;
using System.Diagnostics;
using NUnit.Framework;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.CodeGeneration
{
    public class NUnitListener : IResultListener
    {
        public void ScenarioStarting(Scenario scenario)
        {
            
        }

        public void ScenarioFailed(ScenarioFailureInfo scenarioFailureInfo)
        {
            Debug.Write(scenarioFailureInfo.SuccessPart);
            Debug.WriteLine("{ " + scenarioFailureInfo.FailedPart + " -- FAILED }");
            Assert.Fail(scenarioFailureInfo.Message);
        }

        public void ScenarioPending(ScenarioPendingInfo scenarioPendingInfo)
        {
            var message = "Could not interpret:\r\n" +  scenarioPendingInfo.Line +
                          "\r\nSuggestion:\r\n" + scenarioPendingInfo.Suggestion;
            Debug.WriteLine(message);
            Assert.Ignore(message);
        }

        public void Success(Scenario scenario, string line)
        {
            Debug.WriteLine(line);
        }

        public void ScenarioSucceeded(Scenario scenario)
        {
            
        }

       
        public void Handle(StoryStartingEvent eventToHandle)
        {
            
        }

       
    }
}