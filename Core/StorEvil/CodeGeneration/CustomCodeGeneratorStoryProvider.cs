using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.Parsing;

namespace StorEvil.CodeGeneration
{
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