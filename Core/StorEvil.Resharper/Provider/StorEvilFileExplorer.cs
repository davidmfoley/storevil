using JetBrains.Application;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using StorEvil.Infrastructure;
using StorEvil.Parsing;

namespace StorEvil.Resharper
{
    internal class StorEvilFileExplorer
    {
        public StorEvilFileExplorer(StorEvilTestProvider provider, StorEvilResharperConfigProvider configProvider)
        {
            _provider = provider;
            _configProvider = configProvider;
        }

        private readonly StorEvilTestProvider _provider;
        private StorEvilResharperConfigProvider _configProvider;

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            var config = _configProvider.GetConfigSettingsForProject(psiFile.GetProject());
            var projectFile = psiFile.GetProjectFile();
            var storyReader = new SingleFileStoryReader(new Filesystem(), config, projectFile.Location.ToString());
            var storyProvider = new StoryProvider(storyReader, new StoryParser());

            var stories = storyProvider.GetStories();
            foreach (var story in stories)
            {

                var range = new TextRange(0);
                //UnitTestElementDisposition disposition = new UnitTestElementDisposition(new StorEvilStoryElement(this, Get));
                //consumer(disposition)
            }
        }
    }
}