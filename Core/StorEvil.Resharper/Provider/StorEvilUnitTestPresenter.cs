using JetBrains.CommonControls;
using JetBrains.ReSharper.UnitTestFramework;
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
        }
    }
}