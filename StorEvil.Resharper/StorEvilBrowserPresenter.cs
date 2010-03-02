using System.Drawing;
using JetBrains.CommonControls;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.UI;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;
using JetBrains.ReSharper.Features.Common.TreePsiBrowser;

namespace StorEvil.Resharper
{
    internal class StorEvilBrowserPresenter : TreeModelBrowserPresenter
    {
        internal StorEvilBrowserPresenter()
        {   
            Present<StorEvilUnitTestElement>(PresentTest);
        }

        protected override bool IsNaturalParent(object parentValue,
                                                object childValue)
        {
            return false;
        }

        private static void PresentTest(StorEvilUnitTestElement value,
                                        IPresentableItem item,
                                        TreeModelNode modelNode,
                                        PresentationState state)
        {
            item.RichText = value.GetTitle();

            if (value.IsExplicit)
                item.RichText.SetForeColor(SystemColors.GrayText);

            var stateImage = UnitTestManager.GetStateImage(state);
            var typeImage = UnitTestManager.GetStandardImage(UnitTestElementImage.Test);

            if (stateImage != null)
                item.Images.Add(stateImage);
            else if (typeImage != null)
                item.Images.Add(typeImage);
        }

        protected override object Unwrap(object value)
        {
            if (value is StorEvilUnitTestElement )
                value = ((StorEvilUnitTestElement)value).GetDeclaredElement();

            return base.Unwrap(value);
        }
    }
}