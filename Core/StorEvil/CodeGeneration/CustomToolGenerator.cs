using StorEvil.Parsing;

namespace StorEvil.CodeGeneration
{
    public class FixtureGenerator
    {
        private readonly TestFixtureClassGenerator _generator;

        public FixtureGenerator()           
        {
            _generator = new TestFixtureClassGenerator();
        }

        public string GenerateCode(string inputFilePath, string inputFileContents, string defaultNamespace)
        {
            var story = new StoryParser().Parse(inputFileContents, inputFilePath);

            return _generator.Generate(story, defaultNamespace);
        }
    }
}