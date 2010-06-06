using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using JetBrains.Application;
using JetBrains.CommonControls;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.UI;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;
using JetBrains.Util;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.Parsing;

namespace StorEvil.Resharper
{
    [UnitTestProvider]
    public class StorEvilTestProvider : IUnitTestProvider
    {
        private readonly AssemblyLoader AssemblyLoader = new AssemblyLoader();
        private StorEvilFileExplorer _storEvilFileExplorer;
        private StorEvilAssemblyExplorer _assemblyExplorer;
        private StorEvilResharperConfigProvider _configProvider;

        public StorEvilTestProvider()
        {
            _configProvider = new StorEvilResharperConfigProvider();
            _assemblyExplorer = new StorEvilAssemblyExplorer(this, _configProvider);
            _storEvilFileExplorer = new StorEvilFileExplorer(this, _configProvider);

            AssemblyLoader.RegisterAssembly(typeof(Scenario).Assembly);
            //AssemblyLoader.RegisterPath(Path.GetDirectoryName(typeof(Scenario).Assembly.Location));
        }

        #region IUnitTestProvider Members

        public ProviderCustomOptionsControl GetCustomOptionsControl(ISolution solution)
        {
            return null;
        }

        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo(typeof (StorEvilTaskRunner)) ;
        }

        public string Serialize(UnitTestElement element)
        {
            return "";
        }

        public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
        {
            if (!(element is StorEvilScenarioElement))
                return new List<UnitTestTask>();
            var tasks = new List<UnitTestTask>();
            var storyEl = element.Parent as StorEvilStoryElement;
            var projectEl = storyEl.Parent as StorEvilProjectElement;

            //tasks.Add(new UnitTestTask(null, new AssemblyLoadTask(typeof (Scenario).Assembly.Location)));

            foreach (string assembly in projectEl.Assemblies)
                tasks.Add(new UnitTestTask(null, new AssemblyLoadTask(assembly)));

            tasks.Add(new UnitTestTask(projectEl,
                                       new RunProjectTask(projectEl.GetNamespace().NamespaceName,
                                                          projectEl.Assemblies, explicitElements.Contains(projectEl))));
            tasks.Add(new UnitTestTask(storyEl, new RunStoryTask(storyEl.Id, explicitElements.Contains(storyEl))));
            tasks.Add(new UnitTestTask(element,new RunScenarioTask(((StorEvilScenarioElement) element).Scenario, explicitElements.Contains(element))));
            return tasks;
        }

        public int CompareUnitTestElements(UnitTestElement x, UnitTestElement y)
        {
            if (x is StorEvilStoryElement && y is StorEvilStoryElement)
                return ((StorEvilStoryElement) x).Id == ((StorEvilStoryElement) y).Id ? 0 : -1;

            if (x is StorEvilScenarioElement && y is StorEvilScenarioElement)
                return ((StorEvilScenarioElement) x).Scenario.Id.CompareTo(((StorEvilScenarioElement) y).Scenario.Id);

            if (x is StorEvilProjectElement && y is StorEvilProjectElement)
                return x.GetNamespace().NamespaceName.CompareTo(y.GetNamespace().NamespaceName);

            return -1;
        }

        public string ID
        {
            get { return StorEvilTaskRunner.RunnerId; }
        }

        public string Name
        {
            get { return "StorEvil runner"; }
        }

        public Image Icon
        {
            get { return null; }
        }

        public UnitTestElement Deserialize(ISolution solution, string elementString)
        {
            throw new NotImplementedException();
        }

        public bool IsElementOfKind(UnitTestElement element, UnitTestElementKind elementKind)
        {
            throw new NotImplementedException();
        }

        public void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
            var testElement = element as StorEvilUnitTestElement;
            if (testElement == null)
                return;

            item.RichText = element.ShortName;
        }

        public bool IsElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
        {
            return false;
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            _storEvilFileExplorer.ExploreFile(psiFile, consumer, interrupted);

           
        }

      

        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
            Debug.WriteLine("ExploreExternal");
        }

        public void ExploreAssembly(IMetadataAssembly assembly, IProject project, UnitTestElementConsumer consumer)
        {
            ReadLockCookie.Execute(() => { _assemblyExplorer.ExploreProject(project, consumer); });
        }

        public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
        }

        #endregion
        public bool IsUnitTestStuff(IDeclaredElement element)
        {
            return false;
        }

        public bool IsUnitTestElement(IDeclaredElement element)
        {
            return false;
        }

       
    }

    class StorEvilAssemblyExplorer
    {
        private StorEvilTestProvider _provider;
        private readonly StorEvilResharperConfigProvider _configProvider;

        public StorEvilAssemblyExplorer(StorEvilTestProvider provider, StorEvilResharperConfigProvider configProvider)
        {
            _provider = provider;
            _configProvider = configProvider;
        }

        public void ExploreProject(IProject project, UnitTestElementConsumer consumer)
        {
            var config = _configProvider.GetConfigSettingsForProject(project);

            var projectElement = new StorEvilProjectElement(_provider, null, project, project.Name, config.AssemblyLocations);
            consumer(projectElement);

            if (config == null || config.StoryBasePath == null)
                return;

            var stories = GetStoriesForProject(config);

            foreach (Story story in stories)
                AddStoryElement(config, story, project, consumer, projectElement);
        }

        private IEnumerable<Story> GetStoriesForProject(ConfigSettings config)
        {
            var filesystemStoryReader = new FilesystemStoryReader(new Filesystem(), config);
            var storyProvider = new StoryProvider(filesystemStoryReader, new StoryParser());

            return storyProvider.GetStories();
        }

        private void AddStoryElement(ConfigSettings config, Story story, IProject project,
                                     UnitTestElementConsumer consumer, StorEvilProjectElement parent)
        {
            var storyElement = GetStoryElement(parent, project, story, config);
            consumer(storyElement);

            foreach (IScenario scenario in story.Scenarios)
                AddScenarioElement(project, consumer, storyElement, scenario);
        }

        private StorEvilStoryElement GetStoryElement(StorEvilProjectElement parent, IProject project, Story story, ConfigSettings config)
        {
            return new StorEvilStoryElement(_provider, parent, project, story.Summary, config, story.Id);
        }

        private void AddScenarioElement(IProject project, UnitTestElementConsumer consumer,
                                        StorEvilStoryElement storyElement, IScenario scenario)
        {
            if (scenario is Scenario)
                consumer(new StorEvilScenarioElement(_provider, storyElement, project, scenario.Name, (Scenario)scenario));
            else
                consumer(new StorEvilScenarioOutlineElement(_provider, storyElement, project, scenario.Name,
                                                            (ScenarioOutline)scenario));
        }
    }


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

    internal class StorEvilResharperConfigProvider
    {
        public ConfigSettings GetConfigSettingsForProject(IProject project)
        {
            var reader = new FilesystemConfigReader(new Filesystem(), new ConfigParser());

            return GetConfigForProject(project, reader);
        }

        private static ConfigSettings GetConfigForProject(IProject project, FilesystemConfigReader reader)
        {
            if (project.ProjectFile == null)
                return null;

            var location = project.ProjectFile.ParentFolder.Location;

            if (string.IsNullOrEmpty(location.FullPath))
                return null;

            return reader.GetConfig(location.FullPath);
        }
    }
}