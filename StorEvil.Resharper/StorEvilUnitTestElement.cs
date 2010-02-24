using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestExplorer;

namespace StorEvil.Resharper
{
    public class StorEvilUnitTestElement : UnitTestElement
    {
        private readonly IProject _project;
        private string _title;
        private readonly UnitTestNamespace _namespace = new UnitTestNamespace("namespace.foo");

        public StorEvilUnitTestElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project,  string title) : base(provider, parent)
        {
            _project = project;
            _title = title;
        }

        public override IProject GetProject()
        {
            return _project;
        }

        public override string GetTitle()
        {
            return _title;
        }

        public override string GetTypeClrName()
        {
            return "TypeClrName";
        }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override IList<IProjectFile> GetProjectFiles()
        {
            return new List<IProjectFile>();
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            IDeclaredElement element = GetDeclaredElement();
            if (element != null && element.IsValid())
            {
                var locations = new List<UnitTestElementLocation>();
                foreach (IDeclaration declaration in element.GetDeclarations())
                {
                    IFile file = declaration.GetContainingFile();
                    if (file != null)
                        locations.Add(new UnitTestElementLocation(file.ProjectFile, declaration.GetNameRange(),
                                                                  declaration.GetDocumentRange().TextRange));
                }
                return new UnitTestElementDisposition(locations, this);
            }
            return UnitTestElementDisposition.InvalidDisposition;
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return new StorEvilDeclaredElement();
        }

        public override string GetKind()
        {
            return "Kind";
        }
    }
}