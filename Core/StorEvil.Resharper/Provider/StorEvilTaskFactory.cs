using System.Collections.Generic;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using StorEvil.Resharper.Elements;
using StorEvil.Resharper.Tasks;

namespace StorEvil.Resharper
{
    class StorEvilTaskFactory
    {
        public IList<UnitTestTask> GetTasks(UnitTestElement element, IList<UnitTestElement> explicitElements)
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
            tasks.Add(new UnitTestTask(element, new RunScenarioTask(((StorEvilScenarioElement)element).Scenario, explicitElements.Contains(element))));
            return tasks;
        }
    }
}