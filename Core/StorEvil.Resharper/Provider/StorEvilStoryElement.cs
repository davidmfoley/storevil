using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace StorEvil.Resharper
{
    public class StorEvilStoryElement : StorEvilUnitTestElement
    {
        public ConfigSettings Config { get; set; }
        public string Id { get; set; }
        private readonly UnitTestNamespace _namespace;

        public StorEvilStoryElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project, string title, ConfigSettings config, string id)
            : base(provider, parent, project, title)
        {
            Config = config;
            Id = id;
            _namespace = new UnitTestNamespace(project.Name + " " + title);
        }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override bool Equals(object obj)
        {
            if (obj is StorEvilStoryElement)
            {
                var testElement = (StorEvilStoryElement)obj;
                return testElement.Id == Id;
            }

            return false;
        }

      
    }
}