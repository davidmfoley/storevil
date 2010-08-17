using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Utility;

namespace StorEvil
{
    [TestFixture]
    public class string_utilities
    {
        [Test]
        public void converting_to_csharp_method_name()
        {
            "Foo/Bar Baz".ToCSharpMethodName().ShouldEqual("Foo_Bar_Baz");
        }
    }
}