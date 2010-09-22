using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using StorEvil.Resharper.Elements;

namespace StorEvil.Resharper.Provider
{
    internal class StorEvilElementComparer
    {
        public bool IsOfKind(UnitTestElement element, UnitTestElementKind elementKind)
        {
            if (element is StorEvilScenarioElement)
                return elementKind == UnitTestElementKind.Test;

            if (element is StorEvilStoryElement || element is StorEvilProjectElement || element is StorEvilScenarioOutlineElement)
                return elementKind == UnitTestElementKind.TestContainer;

            return false;
        }

        public int CompareUnitTestElements(UnitTestElement x, UnitTestElement y)
        {
            if (x is StorEvilStoryElement && y is StorEvilStoryElement)
                return ((StorEvilStoryElement)x).Id == ((StorEvilStoryElement)y).Id ? 0 : -1;

            if (x is IStorEvilScenarioElement && y is IStorEvilScenarioElement)
                return ((IStorEvilScenarioElement)x).Id.CompareTo(((IStorEvilScenarioElement)y).Id);

            if (x is StorEvilProjectElement && y is StorEvilProjectElement)
                return x.GetNamespace().NamespaceName.CompareTo(y.GetNamespace().NamespaceName);

            return -1;
        }

        public bool IsDeclaredElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
        {
            
            return false;
        }
    }
}