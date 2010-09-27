using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
using StorEvil.Configuration;
using StorEvil.Infrastructure;


namespace StorEvil.Parsing
{
    [TestFixture]
    public class Single_file_story_reader
    {
        private const string BasePath = "C:\\foo\bar\\baz";

        private IFilesystem FakeFilesystem;
        private SingleFileStoryReader Reader;
        private ConfigSettings Settings;

        [SetUp]
        public void SetupContext()
        {
            FakeFilesystem = MockRepository.GenerateMock<IFilesystem>();
            Settings = new ConfigSettings { StoryBasePath = BasePath };
            Reader = new SingleFileStoryReader(FakeFilesystem, Settings, "foo.feature");

            FakeFilesystem.Stub(x => x.GetFileText("foo.feature")).Return("foo");
        }

        [Test]
        public void when_extension_does_not_match_does_not_return_stories()
        {
            Settings.ScenarioExtensions = new[] {".foo"};

            Reader.GetStoryInfos().Count().ShouldEqual(0);
        }

        [Test]
        public void when_extension_matches_returns_story()
        {
            Settings.ScenarioExtensions = new[] { ".feature" };

            var result = Reader.GetStoryInfos();
            result.Count().ShouldEqual(1);
        }
    }
}