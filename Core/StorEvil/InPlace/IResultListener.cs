using System;
using System.Collections.Generic;
using System.Linq;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    public interface IResultListener
    {
        void StoryStarting(Story story);
        void ScenarioStarting(Scenario scenario);
        void ScenarioFailed(Scenario scenario, string successPart, string failedPart, string message);
        void CouldNotInterpret(Scenario scenario, string line);
        void Success(Scenario scenario, string line);
        void ScenarioSucceeded(Scenario scenario);
        void Finished();
    }

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

    public class ImplementationHelper
    {
        public string Suggest(string s)
        {
            var pieces = s.Split().Where(p=>p.Trim() != "");

            List<string> argTypes = new List<string>();
            var method = pieces.First();
          
            foreach (var piece in pieces.Skip(1))
            {
                if (IsInteger(piece))
                {
                    method += "_arg" + argTypes.Count;
                    argTypes.Add("int");                    
                }
                else if (IsQuotedString(piece))
                {                    
                    method += "_arg" + argTypes.Count;
                    argTypes.Add("string");
                }
                else
                {
                    method += "_" + piece;
                }
            }
                    
            string argText = "";
            int currentArgIndex = 0;
            foreach (var argType in argTypes)
            {
                argText += ", " + argType + " arg" + currentArgIndex;
            }
            argText += " ";
              var code = "public void " + method + "(" + argText.Substring(1).Trim() + ") { }";
            return code;
        }

        private bool IsQuotedString(string piece)
        {
            return piece.StartsWith("\"") && piece.EndsWith("\"");
        }

        private bool IsInteger(string piece)
        {
            return piece.All(c => char.IsDigit(c));
        }
    }    
}