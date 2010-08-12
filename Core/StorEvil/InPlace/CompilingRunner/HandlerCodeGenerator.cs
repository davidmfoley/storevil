using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.InPlace.CompilingRunner
{
    public class HandlerCodeGenerator
    {

        public string GetSourceCode(AssemblyGenerationSpec spec)
        {
            var driverBuilder = new StringBuilder();
            var selectionBuilder = new StringBuilder();

            foreach (var story in spec.Stories)
            {
                var codeBuilder = new StringBuilder();
                codeBuilder.AppendLine("// " + story.Story.Id);
                AppendStoryCode(codeBuilder, story.Story, story.Scenarios.ToArray());
                selectionBuilder.AppendFormat(_driverSelectionClause, story.Story.Id,
                                              story.Story.Id.ToCSharpMethodName());

                var driverCode = string.Format(_driverTemplate, FormatStoryId(story.Story), BuildContextFactorySetup(spec.Assemblies), codeBuilder);
                driverBuilder.AppendLine(driverCode);
            }

            return string.Format(_sourceCodeTemplate, "", driverBuilder.ToString(), selectionBuilder.ToString());
          
        }



        private string FormatStoryId(Story story)
        {
            return story.Id.ToCSharpMethodName();
        }

        private string BuildContextFactorySetup(IEnumerable<string> referencedAssemblies)
        {
            var adds =
                referencedAssemblies.Select(a => string.Format("yield return @\"{0}\";", a));
            return string.Join("\r\n            ", adds.ToArray());
        }

        private void AppendStoryCode(StringBuilder codeBuilder, Story story, Scenario[] scenarios)
        {
            var i = 0;
            foreach (var scenario in scenarios)
            {
                codeBuilder.AppendLine(GetScenarioFunctionBody(i, story, scenario));

                i++;
            }
        }

        private string GetScenarioFunctionBody(int i, Story story, Scenario scenario)
        {
            var scenarioCodeBuilder = new StringBuilder();

            scenarioCodeBuilder.AppendLine(GetScenarioPreamble(i, story));

            foreach (var line in GetLines(scenario))
                scenarioCodeBuilder.AppendLine(BuildLine(line));

            scenarioCodeBuilder.AppendLine(GetScenarioEnd());

            return scenarioCodeBuilder.ToString();
        }

        private string GetScenarioEnd()
        {
            return "}\r\nCollectScenarioResult();";
        }

        private string GetScenarioPreamble(int i, Story story)
        {
            return @"
scenario = scenarios[" + i + @"];

using (StartScenario(story, scenario)) {

#line 1  """ +
                   story.Id + @"""
#line hidden";
        }

        private string BuildLine(ScenarioLine line)
        {
            var lineCodeBuilder = new StringBuilder();

            lineCodeBuilder.AppendFormat(
                @"
if (ShouldContinue) {{
#line {0} 
StorEvilContexts = ExecuteLine(@""{1}"");
#line hidden
}}  
", line.LineNumber,
                line.Text.Replace("\"", "\"\""));
            return lineCodeBuilder.ToString();
        }

        private IEnumerable<ScenarioLine> GetLines(IScenario scenario)
        {
            if (scenario is Scenario)
            {
                var s = (Scenario) scenario;
                return (s.Background ?? new ScenarioLine[0]).Union( s.Body);
            }
            else
            {
                var s = ((ScenarioOutline)scenario).Scenario;
                return (s.Background ?? new ScenarioLine[0]).Union(s.Body);
            }
        }

        private string _driverSelectionClause = @"
            if (story.Id == @""{0}"") {{                
                var handler = new StorEvilDriver_{1}(_bus);
                handler.HandleStory(story);
                _result += handler.GetResult();
                return;
            }}";

        private string _sourceCodeTemplate =
            @"
namespace StorEvilTestAssembly {{
    using System;
    using System.Collections.Generic;
    using StorEvil.Context;
    using StorEvil.Core;
    using StorEvil.Interpreter;
    using StorEvil.InPlace;
    using System.Linq;
    using StorEvil.Parsing;
    using StorEvil.ResultListeners;
    using StorEvil.Events;
    {0}

    {1}

    public class StorEvilDriver : MarshalByRefObject, IStoryHandler {{
        private readonly IEventBus _bus;
        private JobResult _result = new JobResult();

        public StorEvilDriver(IEventBus bus) {{
           _bus = bus;
        }}

        public void HandleStory(Story story) {{                                                
            {2}

            throw new ApplicationException(""StorEvil internal error! Could not handle story with ID: "" + story.Id);
        }}
        public void Finished() {{}}
        public JobResult GetResult() {{ return _result;}}
    }}
}}";

        private string _driverTemplate =
            @"
 public class StorEvilDriver_{0} : StorEvil.InPlace.DriverBase  {{        
        public StorEvilDriver_{0}(IEventBus bus) : base(bus) {{
           
        }}
        
        protected override IEnumerable<string> GetAssemblies() {{
            {1}
        }}

        public override void HandleStory(Story story) {{                                                
            var scenarios = GetScenarios(story);
            var StorEvilContexts = new object[0];
            Scenario scenario;

            {2}

            return;
        }}
    }}";
    }

    public class AssemblyGenerationSpec
    {
        private readonly List<StoryGenerationSpec> _stories = new List<StoryGenerationSpec>();
        public IEnumerable<string> Assemblies { get; set; }

        public void AddStory(Story story, IEnumerable<Scenario> scenarios)
        {
            _stories.Add(new StoryGenerationSpec(story, scenarios));
        }

        public IEnumerable<StoryGenerationSpec> Stories
        {
            get
            {
                return _stories;
            }
        }
    }
    public class StoryGenerationSpec
    {
        public Story Story { get; set; }
        public IEnumerable<Scenario> Scenarios { get; set; }

        public StoryGenerationSpec(Story story, IEnumerable<Scenario> scenarios)
        {
            Story = story;
            Scenarios = scenarios;
        }
    }
}