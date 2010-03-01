using System;
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Application;
using JetBrains.CommonControls;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;

namespace StorEvil.Resharper
{
    [UnitTestProvider]
    public class StorEvilTestProvider : IUnitTestProvider
    {
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

        // This method gets called to generate the tasks that the remote runner will execute
        // When we run all the tests in a class (by e.g. clicking the menu in the margin marker)
        // this method is called with a class element and the list of explicit elements contains
        // one item - the class. We should add all tasks required to prepare to run this class
        // (e.g. loading the assembly and loading the class via reflection - but NOT running the
        // test methods)
        // It is then subsequently called with all method elements, and with the same list (that
        // only has the class as an explicit element). We should return a new sequence of tasks
        // that would allow running the test methods. This will probably be a superset of the class
        // sequence of tasks (e.g. load the assembly, load the class via reflection and a task
        // to run the given method)
        // Because the tasks implement IEquatable, they can be compared to collapse the multiple
        // lists of tasks into a tree (or set of trees) with only one instance of each unique
        // task in order, so in the example, we load the assmbly once, then we load the class once,
        // and then we have multiple tasks for each running test method. If there are multiple classes
        // run, then multiple class lists will be created and collapsed under a single assembly node.
        // If multiple assemblies are run, then there will be multiple top level assembly nodes, each
        // with one or more uniqe class nodes, each with one or more unique test method nodes
        // If you explicitly run a test method from its menu, then it will appear in the list of
        // explicit elements.
        // ReSharper provides an AssemblyLoadTask that can be used to load the assembly via reflection.
        // In the remote runner process, this task is serviced by a runner that will create an app domain
        // shadow copy the files (which we can affect via ProfferConfiguration) and then cause the rest
        // of the nodes (e.g. the classes + methods) to get executed (by serializing the nodes, containing
        // the remote tasks from these lists, over into the new app domain). This is the default way the
        // nunit and mstest providers work. 
        public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
        {
            return new List<UnitTestTask>();
        }

        public int CompareUnitTestElements(UnitTestElement x, UnitTestElement y)
        {
            return 0;
        }

        // Allows us to affect the configuration of a test run, specifying where to start running
        // and whether to shadow copy or not. Interestingly, it looks like we get called even
        // if we don't have any of our kind of tests in this run (so I could affect an nunit run...)
        // I don't know what you can do with UnitTestSession
        public void ProfferConfiguration(TaskExecutorConfiguration configuration, UnitTestSession session)
        {
        }

        public string ID
        {
            get { return "storevil"; }
        }

        public string Name
        {
            get { return "StorEvil R# runner"; }
        }

        public Image Icon
        {
            get { return null; }
        }

        public UnitTestElement Deserialize(ISolution solution, string elementString)
        {
            throw new NotImplementedException();
        }

        public void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
        }

        public bool IsUnitTestStuff(IDeclaredElement element)
        {
            return false;
        }

        public bool IsUnitTestElement(IDeclaredElement element)
        {
            return false;
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            
        }

        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
        }

        // Provides Reflection-like metadata of a physical assembly, called at startup (if the
        // assembly exists) and whenever the assembly is recompiled. It allows us to retrieve
        // the tests that will actually get executed, as opposed to ExploreFile, which is about
        // identifying tests as they are being written, and finding their location in the source
        // code.
        public void ExploreAssembly(IMetadataAssembly assembly, IProject project, UnitTestElementConsumer consumer)
        {
            var parent = new StorEvilUnitTestElement(this, null, project, project.Name + ".AssemblyFoo");

            //consumer(parent);
            //consumer(new StorEvilUnitTestElement(this, parent, project, project.Name + ".Bar1"));
            //consumer(new StorEvilUnitTestElement(this, parent, project, project.Name + ".Bar2"));
        }

        // Called from a refresh of the Unit Test Explorer
        // Allows us to explore the solution, without going into the projects
        public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
            var projects = solution.GetAllProjects();

            var reader = new FilesystemConfigReader(new Filesystem(), new ConfigParser(new Filesystem()));

            foreach (var project in projects)
            {
                if (project.ProjectFile != null)
                {
                    var location = project.ProjectFile.ParentFolder.Location;

                    if (!string.IsNullOrEmpty(location.FullPath))
                    {
                        var config = reader.GetConfig(location.FullPath);

                        if (config != null)
                        {
                            var parent = new StorEvilUnitTestElement(this, null, project, project.Name + " Tests");

                            consumer(parent);
                            //consumer(new StorEvilUnitTestElement(this, parent, project, project.Name + ".Foo"));
                            //consumer(new StorEvilUnitTestElement(this, parent, project, project.Name + ".Bar"));
                        }
                    }
                }
            }
        }
    }
}