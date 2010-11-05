using System;
using System.Drawing;
using JetBrains.ReSharper.Psi;

namespace StorEvil.Resharper.Elements
{
    public class StorEvilDeclaredElementType : DeclaredElementType
    {
        public StorEvilDeclaredElementType(string name) : base(name)
        {

        }

        public override bool IsPresentable(PsiLanguageType language)
        {
            throw new NotImplementedException();
        }

        public override string PresentableName
        {
            get { throw new NotImplementedException(); }
        }

        protected override Image Image
        {
            get { throw new NotImplementedException(); }
        }

        protected override IDeclaredElementPresenter DefaultPresenter
        {
            get { throw new NotImplementedException(); }
        }
    }
}