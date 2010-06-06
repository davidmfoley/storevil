using System;
using System.Collections;
using JetBrains.Application;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using StorEvil.Resharper.Elements;

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
            var psiProject = psiFile.GetProject();
            var project = _environment.GetProject(psiProject.ProjectFile.Location.FullPath);
            
            var stories = project.GetStories(psiFile.GetProjectFile().Location.ToString());
            
            foreach (var story in stories)
            {
                var range = new TextRange(0);
                UnitTestElementDisposition disposition = new UnitTestElementDisposition(
                    new StorEvilStoryElement(_provider, null, psiProject,story.Summary, story.Id ), psiProject.ProjectFile, range, range);
                consumer(disposition);
            }
        }
    }
}