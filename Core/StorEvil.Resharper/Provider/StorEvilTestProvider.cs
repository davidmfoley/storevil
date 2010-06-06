using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using JetBrains.Application;
using JetBrains.CommonControls;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.UI;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;
using StorEvil.Core;

namespace StorEvil.Resharper
{
    [UnitTestProvider]
    public class StorEvilTestProvider : IUnitTestProvider
    {
        private readonly AssemblyLoader AssemblyLoader = new AssemblyLoader();
        private readonly StorEvilAssemblyExplorer _assemblyExplorer;
        private readonly StorEvilResharperConfigProvider _configProvider;
        private readonly StorEvilFileExplorer _storEvilFileExplorer;
        private readonly StorEvilTaskFactory _taskFactory;

        public StorEvilTestProvider()
        {
            _configProvider = new StorEvilResharperConfigProvider();
            _assemblyExplorer = new StorEvilAssemblyExplorer(this, _configProvider);
            _storEvilFileExplorer = new StorEvilFileExplorer(this, _configProvider);
            _taskFactory = new StorEvilTaskFactory();

            AssemblyLoader.RegisterAssembly(typeof (Scenario).Assembly);
            //AssemblyLoader.RegisterPath(Path.GetDirectoryName(typeof(Scenario).Assembly.Location));
        }

        #region IUnitTestProvider Members

        public ProviderCustomOptionsControl GetCustomOptionsControl(ISolution solution)
        {
            return null;
        }

        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo(typeof (StorEvilTaskRunner));
        }

        public string Serialize(UnitTestElement element)
        {
            return "";
        }

        public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
        {
            return _taskFactory.GetTasks(element, explicitElements);
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
}