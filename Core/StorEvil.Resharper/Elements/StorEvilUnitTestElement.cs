using System.Collections.Generic;
using StorEvil.Resharper.Elements;
using IProject = JetBrains.ProjectModel.IProject;
using IProjectFile = JetBrains.ProjectModel.IProjectFile;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Text;

namespace StorEvil.Resharper
{
    public abstract class StorEvilUnitTestElement : UnitTestElement
    {
        protected readonly IProject Project;
        private readonly string _title;

        protected StorEvilUnitTestElement(IUnitTestProvider provider, UnitTestElement parent, IProject project,
                                       string title) : base(provider, parent)
        {
            Project = project;
            _title = title;
        }

        public override IProject GetProject()
        {
            return Project;
        }

        public override string GetTitle()
        {
            return _title;
        }

        public override string GetTypeClrName()
        {
            return GetProject().Name + "." + _title;
        }

        public override IList<IProjectFile> GetProjectFiles()
        {
            return new List<IProjectFile>();
        }

        public override string GetKind()
        {
            return "StorEvil";
        }

        public override string ShortName
        {
            get { return _title; }
        }

        public override bool Matches(string filter, IdentifierMatcher matcher)
        {
            return false;
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            return new UnitTestElementDisposition(new UnitTestElementLocation[0], this);
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return new StorEvilDeclaredElement();
        }

        protected ITypeElement GetDeclaredType()
        {
            return null;
        }

        protected IProjectFile GetProjectFile(string path)
        {
            var item =
                Project.GetSubItems()[0].ParentFolder.FindProjectItemByLocation(new JetBrains.Util.FileSystemPath(path));
            return item as IProjectFile;
        }
    }
}