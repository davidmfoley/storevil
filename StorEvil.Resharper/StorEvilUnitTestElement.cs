using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.ReSharper.UnitTestFramework;

namespace StorEvil.Resharper
{
    public class StorEvilUnitTestElement : UnitTestElement
    {
        private readonly IProject _project;
        private readonly string _title;
        private readonly UnitTestNamespace _namespace = new UnitTestNamespace("namespace.foo");

        public StorEvilUnitTestElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project,  string title) : base(provider, parent)
        {
            _project = project;
            var parentInfo = parent != null ? parent.GetNamespace().NamespaceName + "." : "";
            _title = parentInfo + project.Name + "." + title;

            _namespace = new UnitTestNamespace( project.Name + ".namespaceYo");
        }
        public override string GetHighlighterAttributeId()
        {
            return base.GetHighlighterAttributeId();
        }

        public override ProjectEnvironment? GetProjectEnvironment()
        {
            return base.GetProjectEnvironment();
        }

        public override bool Trackable
        {
            get
            {
                return base.Trackable;
            }
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
            return _project.Name + "." + _title;
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
                        locations.Add(new UnitTestElementLocation(file.ProjectFile, declaration.GetDocumentRange().TextRange,
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


        protected ITypeElement GetDeclaredType()
        {
            return null;

            //var project = GetProject();
            //if (project == null)
            //    return null;

            //var manager = PsiManager.GetInstance(project.GetSolution());

            //var modules = PsiModuleManager.GetInstance(projectEnvoy.Solution).GetPsiModules(project);
            //var projectModule = modules.Count > 0 ? modules[0] : null;

            //using (ReadLockCookie.Create())
            //{
            //    var scope = DeclarationsScopeFactory.ModuleScope(projectModule, false);
            //    var cache = manager.GetDeclarationsCache(scope, true);
            //    return cache.GetTypeElementByCLRName(typeName);
            //}
        }

        public override string GetKind()
        {
            return "StorEvilKind";
        }

        public override string ShortName
        {
            get { return _title; }
        }
            
        public override bool Matches(string filter, JetBrains.Text.IdentifierMatcher matcher)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is StorEvilUnitTestElement)
            {
                var testElement = (StorEvilUnitTestElement)obj;
                return testElement._namespace.NamespaceName == this._namespace.NamespaceName && testElement._title == this._title;
            }

            return false;

        }
    }
}
