using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;

namespace StorEvil.Resharper
{
    public class StorEvilDeclaredElement : IDeclaredElement
    {
        private PsiManager _manager = null;
        private IPsiModule _module = null;

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
            return null;
        }

        public PsiManager GetManager()
        {
          
            return _manager;
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

        public string ShortName
        {
            get { return "ShortName"; }
        }

        public bool CaseSensistiveName
        {
            get { return false; }
        }

        public string XMLDocId
        {
            get { throw new NotImplementedException(); }
        }

        public PsiLanguageType Language
        {
            get { return PsiLanguageType.UNKNOWN; }
        }

        public IPsiModule Module
        {
            get { return _module; }
        }

        public ISubstitution IdSubstitution
        {
            get { return null; }
        }
    }
}