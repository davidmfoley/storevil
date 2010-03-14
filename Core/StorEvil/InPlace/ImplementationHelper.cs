using System.Collections.Generic;
using System.Linq;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    public class ImplementationHelper
    {
        public string Suggest(string s)
        {
            var hasTable = (s.Contains("\r\n|"));
            if (hasTable)
            {
                s = s.Until("\r\n");
            }
            var pieces = s.Split().Where(p => p.Trim() != "");

            var argTypes = new List<string>();
            var method = pieces.First();
            int index = 1;

            foreach (var piece in pieces.Skip(1))
            {
                bool isLastPiece = (++index == pieces.Count());
                if (IsInteger(piece))
                {
                    if (!isLastPiece)
                        method += "_arg" + argTypes.Count;
                    argTypes.Add("int");
                }
                else if (IsQuotedString(piece))
                {
                    if (!isLastPiece)
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
            var code = "public void " + method + "(" + argText.Substring(1).Trim() + ") { StorEvil.ScenarioStatus.Pending(); }";

            return "// " + s + "\r\n" + code;
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