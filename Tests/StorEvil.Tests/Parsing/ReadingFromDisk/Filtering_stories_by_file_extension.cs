using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
using StorEvil.Configuration;
using StorEvil.Infrastructure;

namespace StorEvil.Parsing
{
    [TestFixture, Ignore()]
    public class Reading_stories_with_no_config
    {
        private const string BasePath = "C:\\foo\bar\\baz";

        private IFilesystem FakeFilesystem;

        private FilesystemStoryReader Reader;
        private ConfigSettings Settings;

        private string AStory = "Feature: FooBar\r\nScenario:\r\nFoo Bar";  
        private string NotAStory = "fdsafdsaf fdsafdsa\r\nfgdsafdsafdsafd\r\nafdsafdsar";

        [SetUp]
        public void SetupContext()
        {
            FakeFilesystem = MockRepository.GenerateMock<IFilesystem>();
            Settings = new ConfigSettings {StoryBasePath = BasePath, ScenarioExtensions = new string[] {}};
            Reader = new FilesystemStoryReader(FakeFilesystem, Settings);

            FakeFilesystem.Stub(x => x.GetFilesInFolder(BasePath))
                .Return(new[] { "ignore.txt", "feature.feature" });

            FakeFilesystem.Stub(x => x.GetFileText(BasePath + "\\ignore.txt"))
                .Return(NotAStory);
            FakeFilesystem.Stub(x => x.GetFileText(BasePath + "\\feature.feature"))
                .Return(AStory);
          
            FakeFilesystem.Stub(x => x.GetSubFolders(BasePath))
                .Return(new string[0]);
        }
 
    }
    [TestFixture]
    public class Filtering_stories_by_file_extension
    {
        private const string BasePath = "C:\\foo\bar\\baz";

        private IFilesystem FakeFilesystem;

        private FilesystemStoryReader Reader;
        private ConfigSettings Settings;

        [SetUp]
        public void SetupContext()
        {
            FakeFilesystem = MockRepository.GenerateMock<IFilesystem>();
            Settings = new ConfigSettings {StoryBasePath = BasePath};
            Reader = new FilesystemStoryReader(FakeFilesystem, Settings);

            FakeFilesystem.Stub(x => x.GetFilesInFolder(BasePath))
                .Return(new[] {"ignore.txt", "feature.feature","bar.story"});
            FakeFilesystem.Stub(x => x.GetSubFolders(BasePath))
                .Return(new string[0]);
        }

        [Test]
        public void ignores_files_that_do_not_match_setting_for_extension()
        {
            RunTestWithExtensions(".feature");

            FakeFilesystem.AssertWasNotCalled(x => x.GetFileText("ignore.txt"));
        }

        private void RunTestWithExtensions(params string[] extensionsSettings)
        {
            Settings.ScenarioExtensions = extensionsSettings;
            Reader.GetStoryInfos().ToArray();
        }

        [Test]
        public void reads_files_that_match_setting_for_extension()
        {
            RunTestWithExtensions(".feature");

            FakeFilesystem.AssertWasCalled(x => x.GetFileText("feature.feature"));
        }

        [Test]
        public void when_no_settings_returns_all_files()
        {
            RunTestWithExtensions();

            FakeFilesystem.AssertWasCalled(x => x.GetFileText("feature.feature"));
            FakeFilesystem.AssertWasCalled(x => x.GetFileText("bar.story"));
            FakeFilesystem.AssertWasCalled(x => x.GetFileText("ignore.txt"));
        }

        [Test]
        public void handles_multiple_settings_for_extension()
        {
            RunTestWithExtensions(".feature", ".story");

            FakeFilesystem.AssertWasCalled(x => x.GetFileText("feature.feature"));
            FakeFilesystem.AssertWasCalled(x => x.GetFileText("bar.story"));
        }
    }
}