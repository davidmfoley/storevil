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
using StorEvil.Resharper.Provider;
using StorEvil.Resharper.Runner;

namespace StorEvil.Resharper
{
    [UnitTestProvider]
    public class StorEvilTestProvider : IUnitTestProvider
    {
        private readonly StorEvilAssemblyExplorer _assemblyExplorer;

        private readonly StorEvilFileExplorer _storEvilFileExplorer;
        private readonly StorEvilTaskFactory _taskFactory;
        private readonly StorEvilElementComparer _comparer;
        private readonly StorEvilUnitTestPresenter _presenter;
        private readonly StorEvilTestEnvironment _environment;

        public StorEvilTestProvider()
        {
            _environment = new StorEvilTestEnvironment();
            _assemblyExplorer = new StorEvilAssemblyExplorer(this, _environment);
            _storEvilFileExplorer = new StorEvilFileExplorer(this, _environment);
            _taskFactory = new StorEvilTaskFactory();
            _comparer = new StorEvilElementComparer();
            _presenter = new StorEvilUnitTestPresenter();

        }

        public ProviderCustomOptionsControl GetCustomOptionsControl(ISolution solution)
        {
            return null; 
        }

        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            var runnerInfo = new RemoteTaskRunnerInfo(typeof (StorEvilTaskRunner));            
            return runnerInfo;
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
            return _comparer.CompareUnitTestElements(x, y);           
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
            return _comparer.IsOfKind(element, elementKind);
        }

        public void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
           _presenter.Present(element, item, node, state);          
        }

        public bool IsElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
        {
            return _comparer.IsDeclaredElementOfKind(declaredElement, elementKind);
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
    }
}