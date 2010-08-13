using System.Drawing;
using JetBrains.CommonControls;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.UI;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;

namespace StorEvil.Resharper
{
    internal class StorEvilUnitTestPresenter
    {
        public void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
            var testElement = element as StorEvilUnitTestElement;
            if (testElement == null)
                return;

            item.RichText = element.ShortName;
            
            Image standardImage = UnitTestManager.GetStandardImage(UnitTestElementImage.Test);
            Image stateImage = UnitTestManager.GetStateImage(state);
            if (stateImage != null)
            {
                item.Images.Add(stateImage);
            }
            else if (standardImage != null)
            {
                item.Images.Add(standardImage);
            }
        }
    }
}