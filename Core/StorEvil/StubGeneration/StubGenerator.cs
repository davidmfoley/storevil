using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.Interpreter;

namespace StorEvil.StubGeneration
{
    public class StubGenerator : IStoryHandler
    {
        private readonly ScenarioInterpreter _scenarioInterpreter;
        private readonly ImplementationHelper _implementationHelper;
        public readonly ITextWriter SuggestionWriter;
        private readonly List<string> _suggestions = new List<string>();

        public StubGenerator(ScenarioInterpreter scenarioInterpreter, ImplementationHelper implementationHelper, ITextWriter suggestionWriter)
        {
            _scenarioInterpreter = scenarioInterpreter;
            SuggestionWriter = suggestionWriter;
            _implementationHelper = implementationHelper;
        }

        public void HandleStory(Story story, StoryContext context)
        {
            foreach (var scenario in story.Scenarios)
            {
                var scenarioContext = context.GetScenarioContext();
                foreach (var line in GetLines(scenario))
                {
                    if (null == _scenarioInterpreter.GetChain(scenarioContext, line))
                    {
                        var sugesstedCode = _implementationHelper.Suggest(line) + "\r\n";
                        if (!AlreadyHaveSuggestion(sugesstedCode))
                        {
                            
                            _suggestions.Add(sugesstedCode);
                        }
                    }
                }
            }
        }

        private bool AlreadyHaveSuggestion(string suggestedCode)
        {
            return _suggestions.Select(s=>ExtractDeclaration(s)).Any(x => x == ExtractDeclaration(suggestedCode));
        }

        private string ExtractDeclaration(string suggestedCode)
        {
            return suggestedCode.Split(new[] { "\r\n"}, StringSplitOptions.None)[1];
        }

        private IEnumerable<string> GetLines(IScenario scenario)
        {
            if (scenario is Scenario)
                return ((Scenario) scenario).Body;

            return ((ScenarioOutline) scenario).Scenario.Body;
        }

        public void Finished()
        {
            var joined = "        " + string.Join("\r\n", _suggestions.ToArray()).Replace("\r\n", "\r\n        ");
            
            string classFormat = @"

namespace Your.Namespace.Here 
{{
    [StorEvil.Context]
    public class StubContextClass 
    {{
{0}
    }}
}}";
            var classDefinition = string.Format(classFormat, joined);
            SuggestionWriter.Write(classDefinition);

        }
    }

    public class ClipboardWriter : ITextWriter
    {
        public void Write(string suggestions)
        {
            Clipboard.SetText(suggestions);
        }
    }

  
}