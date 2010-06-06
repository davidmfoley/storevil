using System;
using System.Collections;
using JetBrains.Application;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace StorEvil.Resharper
{
    internal class StorEvilFileExplorer
    {
        public StorEvilFileExplorer(StorEvilTestProvider provider, StorEvilTestEnvironment environment)
        {
            _provider = provider;
            _environment = environment;
        }

        private readonly StorEvilTestProvider _provider;

        private StorEvilTestEnvironment _environment;

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            var project = _environment.GetProject(psiFile.GetProject());
            
            var stories = project.GetStories(psiFile.GetProjectFile().Location.ToString());
            
            foreach (var story in stories)
            {
                var range = new TextRange(0);
                //UnitTestElementDisposition disposition = new UnitTestElementDisposition(new StorEvilStoryElement(this, Get));
                //consumer(disposition)
            }
        }
    }
}