using System;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.ResultListeners
{
    public class ConsoleResultListener : IResultListener
    {
        public void StoryStarting(Story story)
        {
            ColorWrite(ConsoleColor.White, "\r\n" + "\r\nSTORY: \r\n" + story.Summary);
        }

        public void ScenarioStarting(Scenario scenario)
        {
            ColorWrite(ConsoleColor.White, "\r\n" + scenario.Name);
        }

        public void ScenarioFailed(ScenarioFailureInfo scenarioFailureInfo)
        {
            ColorWrite(ConsoleColor.Yellow, scenarioFailureInfo.SuccessPart);
            ColorWrite(ConsoleColor.Red, " " + scenarioFailureInfo.FailedPart + "\r\nFAILED\r\n", scenarioFailureInfo.Message + "\r\n");
        }

        public void ScenarioPending(ScenarioPendingInfo scenarioPendingInfo)
        {
            var message = scenarioPendingInfo.CouldNotInterpret ?  "Could not interpret" : "Pending";
            ColorWrite(ConsoleColor.Yellow, scenarioPendingInfo.Line + " -- " + message);

            if (scenarioPendingInfo.CouldNotInterpret && scenarioPendingInfo.Suggestion != null)
                ColorWrite(ConsoleColor.Gray, scenarioPendingInfo.Suggestion);
        }

        public void Success(Scenario scenario, string line)
        {
            ColorWrite(ConsoleColor.Green, line);
        }

        public void ScenarioSucceeded(Scenario scenario)
        {
            
        }

        public void Finished()
        {
            
        }

        private void ColorWrite(ConsoleColor color, params string[] lines)
        {
            if (ColorEnabled)
                Console.ForegroundColor = color;

            foreach (var s in lines)
                Console.WriteLine(s);

            if (ColorEnabled)
                Console.ResetColor();
        }

        public bool ColorEnabled { get; set; }
    }
}