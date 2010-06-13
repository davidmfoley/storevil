using StorEvil.Parsing;

namespace StorEvil.CodeGeneration
{
    public class FixtureGenerator
    {
        private CustomToolCodeGenerator _generator;

        public FixtureGenerator()           
        {
            _generator = new CustomToolCodeGenerator();
        }

        public string GenerateCode(string inputFilePath, string inputFileContents)
        {
            var story = new StoryParser().Parse(inputFileContents, inputFilePath);

            return _generator.Generate(story);
        }
    }
}