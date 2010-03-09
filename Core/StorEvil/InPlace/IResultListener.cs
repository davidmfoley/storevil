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

    public class ImplementationHelper
    {
        public string Suggest(string s)
        {
            var hasTable = (s.Contains("\r\n|"));
            if (hasTable)
            {
                s = s.Until("\r\n");
            }
            var pieces = s.Split().Where(p=>p.Trim() != "");

            var argTypes = new List<string>();
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

            if (hasTable)
                argText += ", string[][] tableData";

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