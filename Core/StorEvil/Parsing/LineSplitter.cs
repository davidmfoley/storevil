using System.Collections.Generic;
using StorEvil.Core;

namespace StorEvil.Parsing
{
    public class LineSplitter
    {
        public  IEnumerable<ScenarioLine> Split(string text)
        {
            var currentPosition = 0;
            var lineNumber = 0;

            while(currentPosition > -1 && currentPosition < text.Length)
            {
                var nextLinebreak = text.IndexOf('\n', currentPosition);
                if (nextLinebreak == -1)
                    nextLinebreak = text.Length;

                var substring = text.Substring(currentPosition, nextLinebreak - currentPosition).TrimEnd('\r');
               
                yield return
                    new ScenarioLine
                        {
                            LineNumber = ++lineNumber,
                            StartPosition = currentPosition,
                            Text = substring
                        };

                currentPosition = nextLinebreak + 1;
            }           
        }
    }
}