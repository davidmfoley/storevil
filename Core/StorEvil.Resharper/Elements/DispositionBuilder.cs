using System.IO;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using StorEvil.Core;

namespace StorEvil.Resharper.Elements
{
    public class DispositionBuilder
    {
        public static UnitTestElementDisposition BuildDisposition(UnitTestElement element, ScenarioLocation location, IProjectFile  projectFile)
        {
            var contents = File.ReadAllText(location.Path);
            var range = new TextRange(LineToOffset(contents, location.FromLine), LineToOffset(contents, location.ToLine));

            return new UnitTestElementDisposition(element, projectFile, range, new TextRange(0));
        }

        private static int LineToOffset(string contents, int lineNumber)
        {
            var line = 1;

            for (int i = 0; i < contents.Length; i++)
            {
                if (line >= lineNumber)
                    return i;

                if (contents[i] == '\n')
                    line++;
            }

            return 0;
        }
    }
}