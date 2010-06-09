using System.Text;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.Parsing;
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

    public class CustomToolCodeGenerator
    {
        public string Generate(Story story)
        {
            var stringBuilder = new StringBuilder();
            var fixtureName = story.Id.ToCSharpMethodName();
            stringBuilder.Append("[NUnit.Framework.TestFixtureAttribute] public class " + fixtureName  + " { ");

            foreach (var scenario in story.Scenarios)
            {
                var name = scenario.Name.ToCSharpMethodName();
                stringBuilder.AppendLine("[NUnit.Framework.TestAttribute] public void " + name + "() { }");                
            }
            stringBuilder.Append("}");

            return stringBuilder.ToString();
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