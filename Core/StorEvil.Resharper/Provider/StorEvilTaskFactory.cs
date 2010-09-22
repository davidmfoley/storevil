using System;
using System.Collections.Generic;
using JetBrains.ReSharper.UnitTestFramework;
using StorEvil.Core;
using StorEvil.Parsing;
using StorEvil.Resharper.Elements;
using StorEvil.Resharper.Tasks;

namespace StorEvil.Resharper
{
    class StorEvilTaskFactory
    {
      

        public IList<UnitTestTask> GetTasks(UnitTestElement element, IList<UnitTestElement> explicitElements)
        {
            //if (!(element is StorEvilScenarioElement))
            //    return new List<UnitTestTask>();

            var tasks = new List<UnitTestTask>();

            if (element is StorEvilProjectElement)
            {
                var projectEl = element as StorEvilProjectElement;
                tasks.Add(new UnitTestTask(null, new LoadContextAssemblyTask(typeof(Scenario).Assembly.Location)));

                foreach (string assembly in projectEl.Assemblies)
                {
                    tasks.Add(new UnitTestTask(null, new LoadContextAssemblyTask(assembly)));
                }
                tasks.Add(GetProjectTask(projectEl, explicitElements));
            }

            if (element is StorEvilStoryElement)
            {
                tasks.Add(GetProjectTask(element.Parent as StorEvilProjectElement, explicitElements));
                tasks.Add(GetStoryTask(element as StorEvilStoryElement, explicitElements));
            }

            if (element is StorEvilScenarioElement)
            {                
                if (element.Parent is StorEvilScenarioOutlineElement)
                {
                    tasks.Add(GetProjectTask(element.Parent.Parent.Parent as StorEvilProjectElement, explicitElements));
                    tasks.Add(GetStoryTask(element.Parent.Parent as StorEvilStoryElement, explicitElements));                    
                    tasks.Add(GetScenarioOutlineTask(element.Parent, explicitElements));
                    tasks.Add(GetScenarioTask(element, explicitElements));
                }
                else
                {
                    tasks.Add(GetProjectTask(element.Parent.Parent as StorEvilProjectElement, explicitElements));
                    tasks.Add(GetStoryTask(element.Parent as StorEvilStoryElement, explicitElements));
                    tasks.Add(GetScenarioTask(element, explicitElements));
                }
            }

            if (element is StorEvilScenarioOutlineElement)
            {
                tasks.Add(GetProjectTask(element.Parent.Parent as StorEvilProjectElement, explicitElements));
                tasks.Add(GetStoryTask(element.Parent as StorEvilStoryElement, explicitElements));
                tasks.Add(GetScenarioOutlineTask(element, explicitElements));               
            }

         
            return tasks;
        }

        

        private UnitTestTask GetProjectTask(StorEvilProjectElement projectEl, IList<UnitTestElement> explicitElements)
        {
            return new UnitTestTask(projectEl,
                                    new RunProjectTask(projectEl.GetNamespace().NamespaceName,
                                                       projectEl.Assemblies, explicitElements.Contains(projectEl)));
        }

        private UnitTestTask GetScenarioTask(UnitTestElement element, IList<UnitTestElement> explicitElements)
        {
            return new UnitTestTask(element,
                                    new RunScenarioTask(((StorEvilScenarioElement) element).Scenario as Scenario,
                                                        explicitElements.Contains(element)));
        }

        private UnitTestTask GetScenarioOutlineTask(UnitTestElement element, IList<UnitTestElement> explicitElements)
        {
            
            return new UnitTestTask(element,
                                   new RunScenarioOutlineTask(((StorEvilScenarioOutlineElement)element).ScenarioOutline,
                                                       explicitElements.Contains(element)));
        }

        private UnitTestTask GetStoryTask(StorEvilStoryElement storyEl, IList<UnitTestElement> explicitElements)
        {
            return new UnitTestTask(storyEl, new RunStoryTask(storyEl.Id, explicitElements.Contains(storyEl)));
        }
    }
}