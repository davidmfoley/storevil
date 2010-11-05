using System.Collections.Generic;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;

namespace StorEvil.Resharper.Elements
{
    public class StorEvilDeclaredElement : IDeclaredElement
    {
        public IList<IDeclaration> GetDeclarations()
        {
            return new List<IDeclaration>();
        }

        public IList<IDeclaration> GetDeclarationsIn(IProjectFile projectFile)
        {
            return new List<IDeclaration>();
        }

        public ISearchDomain GetSearchDomain()
        {
            return null;
        }

        public DeclaredElementType GetElementType()
        {
            return new StorEvilDeclaredElementType("test");
        }

        public PsiManager GetManager()
        {
            return null;
        }

        public ITypeElement GetContainingType()
        {
            return null;
        }

        public ITypeMember GetContainingTypeMember()
        {
            return null;
        }

        public XmlNode GetXMLDoc(bool inherit)
        {
            return null;
        }

        public XmlNode GetXMLDescriptionSummary(bool inherit)
        {
            return null;
        }

        public bool IsValid()
        {
            return true;
        }

        public bool IsSynthetic()
        {
            return true;
        }

        public IList<IProjectFile> GetProjectFiles()
        {
            return new List<IProjectFile>();
        }

        public bool HasDeclarationsInProjectFile(IProjectFile projectFile)
        {
            return false;
        }

        public string ShortName { get; private set; }

        public bool CaseSensistiveName { get; private set; }

        public PsiLanguageType Language { get; private set; }

        public IPsiModule Module { get; private set; }

        public ISubstitution IdSubstitution { get; private set; }
    }
}