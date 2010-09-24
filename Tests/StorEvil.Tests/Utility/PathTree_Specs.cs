using NUnit.Framework;
using StorEvil.Assertions;

namespace StorEvil.Utility
{
    public class PathTree_Specs
    {
        protected PathTree PathTree;


        [TestFixture]
        public class Empty_tree : PathTree_Specs
        {
            [SetUp]
            public void SetUpContext()
            {
                PathTree = new PathTree();
            }

            [Test]
            public void count_is_zero()
            {
                PathTree.Count.ShouldEqual(0);

            }

            [Test]
            public void Root_is_an_empty_node()
            {
                var count = this.PathTree.Root.Children.Count;
                count.ShouldEqual(0);
            }
        }

        [TestFixture]
        public class Adding_nodes : PathTree_Specs
        {
            [SetUp]
            public void SetUpContext()
            {
                PathTree = new PathTree();
                PathTree.Put(new[] { "foo", "bar" }, "foo/bar");
            }

            [Test]
            public void Can_retrieve_node()
            {
                var o = PathTree.Get(new[] { "foo", "bar" });
                o.ShouldEqual("foo/bar");
            }

            [Test]
            public void returns_null_for_unknown_sub_path()
            {
                PathTree.Get(new[] { "foo", "bar", "baz" }).ShouldBe(null);
            }

            [Test]
            public void returns_null_for_completely_unknown_path()
            {
                PathTree.Get(new[] { "bar", "baz" }).ShouldBe(null);
            }
        }
    }
}