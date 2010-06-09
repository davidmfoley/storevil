using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Parsing;
using StorEvil.ResultListeners;
using StorEvil.Utility;

namespace StorEvil.CodeGeneration
{
    public class CustomTool
    {
        private readonly CustomToolCodeGenerator _generator;
        private readonly CustomCodeGeneratorStoryProvider _provider;

        public CustomTool(CustomCodeGeneratorStoryProvider provider, CustomToolCodeGenerator generator)
        {
            _provider = provider;
            _generator = generator;
        }

        public string GenerateCode(string inputFilePath)
        {
            return "";
        }
    }

    public class TestFixture
    {
        private StoryContext _storyContext;
        private ScenarioContext _scenarioContext;
        private StandardScenarioInterpreter _interpreter;
        private ScenarioLineExecuter _scenarioLineExecuter;
        private IResultListener _listener;
        private SessionContext _sessionContext;
        private Scenario _currentScenario;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _listener = GetListener();
            _storyContext = GetSessionContext().GetContextForStory();
            _interpreter = new StandardScenarioInterpreter();
            _scenarioLineExecuter = new ScenarioLineExecuter(_interpreter, _listener);
        }

        private IResultListener GetListener()
        {
            return new NUnitListener();
        }

        private ISessionContext GetSessionContext()
        {
            if (_sessionContext == null)
                _sessionContext = new SessionContext();

            return _sessionContext;
        }

        [SetUp]
        public void SetUpContext()
        {
            _scenarioContext = _storyContext.GetScenarioContext();
         
        }

        protected void ExecuteLine(string line)
        {
            _scenarioLineExecuter.ExecuteLine(_currentScenario, _scenarioContext, line);
          
        }

        protected void SetCurrentScenario(string id, string summary)
        {
            _currentScenario = new Scenario(id, summary, new ScenarioLine[0]);
        }

        [TearDown]
        public void TearDown()
        {
            _scenarioContext.Dispose();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            _storyContext.Dispose();
        }
    }

    internal class NUnitListener : IResultListener
    {
        public void StoryStarting(Story story)
        {
            
        }

        public void ScenarioStarting(Scenario scenario)
        {
            
        }

        public void ScenarioFailed(ScenarioFailureInfo scenarioFailureInfo)
        {
            Debug.Write(scenarioFailureInfo.SuccessPart);
            Debug.WriteLine("{ " + scenarioFailureInfo.FailedPart + " -- FAILED }");
            Assert.Fail(scenarioFailureInfo.Message);
        }

        public void ScenarioPending(ScenarioPendingInfo scenarioPendingInfo)
        {
            var message = "Could not interpret:\r\n" +  scenarioPendingInfo.Line +
                "\r\nSuggestion:\r\n" + scenarioPendingInfo.Suggestion;
            Assert.Ignore(message);
        }

        public void Success(Scenario scenario, string line)
        {
            Debug.WriteLine(line);
        }

        public void ScenarioSucceeded(Scenario scenario)
        {
            
        }

        public void Finished()
        {
            
        }
    }

    public class CustomToolCodeGenerator
    {
        public string Generate(Story story)
        {
            var stringBuilder = new StringBuilder();
            var fixtureName = story.Id.ToCSharpMethodName();
            stringBuilder.Append("[NUnit.Framework.TestFixtureAttribute] public class " + fixtureName + " : StorEvil.CodeGeneration.TestFixture {\r\n ");

            foreach (var scenario in story.Scenarios)
            {
                var name = scenario.Name.ToCSharpMethodName();
                stringBuilder.AppendLine("[NUnit.Framework.TestAttribute] public void " + name + "() {\r\n" + GetBody(scenario) +  " }");                
            }
            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }

        private string GetBody(IScenario scenario)
        {
            if (scenario is Scenario)
                return GetScenarioBody((Scenario) scenario);
            return null;
        }

        private string GetScenarioBody(Scenario scenario)
        {
            var lines = scenario.Body.Select((l, i) => GetScenarioLine(l));

            return string.Join("\r\n", lines.ToArray());
        }

        private string GetScenarioLine(ScenarioLine scenarioLine)
        {
            const string fmt = "#line {0}\r\nExecuteLine(@\"{1}\");\r\n#line hidden\r\n";
            return string.Format(fmt, scenarioLine.LineNumber, scenarioLine.Text.Replace("\"", "\"\""));
        }
    }

    public class CustomCodeGeneratorStoryProvider
    {
        private readonly FilesystemConfigReader _configReader;
    
        public CustomCodeGeneratorStoryProvider(FilesystemConfigReader configReader)
        {
            _configReader = configReader;                      
        }

        public Story GetStory(string fileName)
        {
            var settings = _configReader.GetConfig(fileName);
            var reader = new SingleFileStoryReader(new Filesystem(), settings, fileName);
            var provider = new StoryProvider(reader, new StoryParser());

            return null;
        }
    }
}