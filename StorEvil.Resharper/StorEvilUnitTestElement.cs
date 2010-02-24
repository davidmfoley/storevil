using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestExplorer;

namespace StorEvil.Resharper
{
    public class StorEvilUnitTestElement : UnitTestElement
    {
        private string _title;
        private UnitTestNamespace _namespace = new UnitTestNamespace("");

        public StorEvilUnitTestElement(StorEvilTestProvider provider, UnitTestElement parent) : base(provider, parent)
        {
           
        }

        public override IProject GetProject()
        {
            return null;
        }

        public override string GetTitle()
        {
            return _title;
        }

        public override string GetTypeClrName()
        {
            return "";
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
            return null;
        }

        public override string GetKind()
        {
            return "";
        }
    }
}