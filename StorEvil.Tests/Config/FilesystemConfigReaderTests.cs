using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace StorEvil.Config
{
    [TestFixture]
    public class FilesystemConfigReaderTests
    {
        private FilesystemConfigReader FilesystemConfigReader;

        private IFilesystem FakeFilesystem;
        private IConfigFileReader FakeParser;

        private const string FakeConfigFileContents = "foo";

        [SetUp]
        public void SetupContext()
        {
            FakeFilesystem = MockRepository.GenerateMock<IFilesystem>();
            FakeParser = MockRepository.GenerateMock<IConfigFileReader>();

            FilesystemConfigReader = new FilesystemConfigReader(FakeFilesystem, FakeParser);
        }

        [Test]
        public void when_config_present_in_same_folder_uses_it()
        {
            const string path = "c:\\test\\storevil.config";

            FileExistsWithTestContents(path);

            var settings = new ConfigSettings();
            ParserReturnsConfigSettings(settings);

            var result = FilesystemConfigReader.GetConfig("c:\\test\\");

            Assert.That(result, Is.SameAs(settings));
        }

        [Test]
        public void when_no_config_present_in_same_folder_searches_up_the_tree()
        {
            FileExistsWithTestContents("c:\\test\\storevil.config");

            var settings = new ConfigSettings();
            ParserReturnsConfigSettings(settings);

            // note: should make successive calls up the directory structure
            // first \test\foo\bar, then \test\foo, then \test
            var result = FilesystemConfigReader.GetConfig("c:\\test\\foo\\bar\\");

            Assert.That(result, Is.SameAs(settings));
        }

        [Test]
        public void when_no_config_present_in_tree_returns_default()
        {
            const string path = "c:\\test\\storevil.config";

            var result = FilesystemConfigReader.GetConfig(path);
            Assert.That(result, Is.Not.Null);
        }

        private void FileExistsWithTestContents(string path)
        {
            FileExists(path);
            FilesystemReturnsConfigFileContents(path);
        }

        private void FileExists(string path)
        {
            FakeFilesystem.Stub(x => x.FileExists(path)).Return(true);
        }

        private void FilesystemReturnsConfigFileContents(string path)
        {
            FakeFilesystem.Stub(x => x.GetFileText(path)).Return(FakeConfigFileContents);
        }

        private void ParserReturnsConfigSettings(ConfigSettings settings)
        {
            FakeParser.Stub(x => x.Read(FakeConfigFileContents)).Return(settings);
        }
    }
}