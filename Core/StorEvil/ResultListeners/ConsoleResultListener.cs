using System;
using StorEvil.Core;

namespace StorEvil.InPlace
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

        public void ScenarioFailed(Scenario scenario, string successPart, string failedPart, string message)
        {
            ColorWrite(ConsoleColor.Yellow, successPart);
            ColorWrite(ConsoleColor.Red, " " + failedPart + "\r\nFAILED\r\n", message + "\r\n");
        }

        public void CouldNotInterpret(Scenario scenario, string line)
        {
            ColorWrite(ConsoleColor.Yellow, line + " -- Could not interpret");
            ColorWrite(ConsoleColor.Gray, new ImplementationHelper().Suggest(line));
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