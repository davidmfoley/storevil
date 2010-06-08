using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.Parsing;

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
            return "using NUnit.Framework; [TestFixture] public class foo {} ";
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
            var provider = new StoryProvider(new SingleFileStoryReader(new Filesystem(), settings, fileName),
                                             new StoryParser());

            return null;
        }
    }
}