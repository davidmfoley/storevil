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
    public class StorEvilTypeElement : ITypeElement
    {
        private IPsiModule _module;
        private PsiManager _manager;

        public StorEvilTypeElement(IPsiModule module, PsiManager manager)
        {
            _module = module;
            _manager = manager;
        }

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
            throw new NotImplementedException();
        }

        public ITypeElement GetContainingType()
        {
            throw new NotImplementedException();
        }

        public ITypeMember GetContainingTypeMember()
        {
            throw new NotImplementedException();
        }

        public XmlNode GetXMLDoc(bool inherit)
        {
            throw new NotImplementedException();
        }

        public XmlNode GetXMLDescriptionSummary(bool inherit)
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public bool IsSynthetic()
        {
            throw new NotImplementedException();
        }

        public IList<IProjectFile> GetProjectFiles()
        {
            throw new NotImplementedException();
        }

        public bool HasDeclarationsInProjectFile(IProjectFile projectFile)
        {
            throw new NotImplementedException();
        }

        public string ShortName
        {
            get { throw new NotImplementedException(); }
        }

        public bool CaseSensistiveName
        {
            get { throw new NotImplementedException(); }
        }

        public PsiLanguageType Language
        {
            get { throw new NotImplementedException(); }
        }

        public IPsiModule Module
        {
            get { throw new NotImplementedException(); }
        }

        public ISubstitution IdSubstitution
        {
            get { throw new NotImplementedException(); }
        }

        public ITypeParameter[] TypeParameters
        {
            get { throw new NotImplementedException(); }
        }

        public IList<IAttributeInstance> GetAttributeInstances(bool inherit)
        {
            throw new NotImplementedException();
        }

        public IList<IAttributeInstance> GetAttributeInstances(CLRTypeName clrName, bool inherit)
        {
            throw new NotImplementedException();
        }

        public bool HasAttributeInstance(CLRTypeName clrName, bool inherit)
        {
            throw new NotImplementedException();
        }

        public IList<IDeclaredType> GetSuperTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITypeMember> GetMembers()
        {
            throw new NotImplementedException();
        }

        public INamespace GetContainingNamespace()
        {
            throw new NotImplementedException();
        }

        public string CLRName
        {
            get { throw new NotImplementedException(); }
        }

        public IList<ITypeElement> NestedTypes
        {
            get { throw new NotImplementedException(); }
        }

        public IList<IConstructor> Constructors
        {
            get { throw new NotImplementedException(); }
        }

        public IList<IOperator> Operators
        {
            get { throw new NotImplementedException(); }
        }

        public IList<IMethod> Methods
        {
            get { throw new NotImplementedException(); }
        }

        public IList<IProperty> Properties
        {
            get { throw new NotImplementedException(); }
        }

        public IList<IEvent> Events
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<string> MemberNames
        {
            get { throw new NotImplementedException(); }
        }
    }
}