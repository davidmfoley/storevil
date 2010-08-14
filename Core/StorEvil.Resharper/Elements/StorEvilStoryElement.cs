using System.IO;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using StorEvil.Configuration;

namespace StorEvil.Resharper.Elements
{
    public class StorEvilStoryElement : StorEvilUnitTestElement
    {
        private readonly string _path;
        public string Id { get; set; }
        private readonly UnitTestNamespace _namespace;

        public StorEvilStoryElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project, string title, string path)
            : base(provider, parent, project, title)
        {
            _path = path;

            _namespace = new UnitTestNamespace(project.Name + " " + title);
        }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as StorEvilStoryElement);
        }

        public bool Equals(StorEvilStoryElement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other._namespace, _namespace) && Equals(other.Id, Id);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ (_namespace != null ? _namespace.GetHashCode() : 0);
                result = (result*397) ^ (Id != null ? Id.GetHashCode() : 0);
                return result;
            }
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            var projectFile = GetProjectFile(_path);

            var range = new TextRange(0, 0);
            var location = new UnitTestElementLocation(projectFile, range, range);

            var unitTestElementLocations = new[] { location }; //new UnitTestElementLocation(), .Id};
            return new UnitTestElementDisposition(unitTestElementLocations, this);
        }

        
    }
}