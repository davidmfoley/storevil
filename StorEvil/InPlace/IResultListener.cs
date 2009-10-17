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

    }

    public class ConsoleResultListener : IResultListener
    {
        public void StoryStarting(Story story)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\r\n STORY: \r\n" + story.Summary);
            Console.ResetColor();
        }

        public void ScenarioStarting(Scenario scenario)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\r\n" + scenario.Name);
            Console.ResetColor();
        }

        public void ScenarioFailed(Scenario scenario, string successPart, string failedPart, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(successPart);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" " + failedPart + "\r\nFAILED\r\n" );
            Console.WriteLine(message + "\r\n");
            Console.ResetColor();
        }

        public void CouldNotInterpret(Scenario scenario, string line)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(line + " -- Could not interpret");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(new ImplementationHelper().Suggest(line));
            Console.ResetColor();
        }

        public void Success(Scenario scenario, string line)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(line);
            Console.ResetColor();
        }
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