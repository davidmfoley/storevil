using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [TestFixture]
    public class RelativePathHelper
    {
        [Test]
        public void returns_empty_for_same_path()
        {
            PathHelper.GetRelativePathPieces("C:\\foo", "C:\\foo").Length.ShouldEqual(0);
        }

        [Test]
        public void returns_a_single_piece()
        {
            var result = PathHelper.GetRelativePathPieces("C:\\foo", "C:\\foo\\bar");
            result.ElementsShouldEqual("bar");
        }

        [Test]
        public void handles_going_up_tree()
        {
            var result = PathHelper.GetRelativePathPieces("C:\\foo\\bar", "C:\\foo\\baz");
            result.ElementsShouldEqual("..", "baz");
        }
    }

    [TestFixture]
    public class Splitting_a_path
    {
        [Test]
        public void can_split()
        {
            PathHelper.SplitPath("C:\\foo\\bar\\baz").ElementsShouldEqual("C:\\", "foo", "bar", "baz");
        }
    }


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
                PathTree.Root.Children.Count().ShouldEqual(0);
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

    [TestFixture]
    public class Transforming_a_path_tree
    {
        private Hashtable ResultHash;

        [SetUp]
        public void SetUpContext()
        {
            var tree = new PathTree();
            tree.Put(new[] { "foo", "bar", "1" }, "item 1");
            tree.Put(new[] { "foo", "bar", "baz", "2" }, "item 2");
            tree.Put(new[] { "foo", "3" }, "item 3");
            tree.Put(new[] { "4" }, "item 4");

            var result = tree.Transform(BuildBranch, BuildLeaf);
            ResultHash = ((KeyValuePair<string, object>)result).Value as Hashtable;
        }

        [Test]
        public void should_have_top_level_item()
        {
            ResultHash["4"].ShouldEqual("item 4");
        }

        [Test]
        public void should_have_nested_items()
        {
           
            var fooHash = (Hashtable)ResultHash["foo"];
            fooHash["3"].ShouldBe("item 3");
            var barHash = (Hashtable) fooHash["bar"];

            barHash["1"].ShouldBe("item 1");
            var bazHash = (Hashtable)barHash["baz"];
            bazHash["2"].ShouldBe("item 2");
        }

        private object BuildLeaf(string pathPiece, object item)
        {
            return new KeyValuePair<string, object>(pathPiece, item);
        }

        private object BuildBranch(string pathPiece, object[] children)
        {
            var h = new Hashtable();

            foreach (var child in children)
            {
                var kvp = (KeyValuePair<string, object>) child;
                h.Add(kvp.Key, kvp.Value);
            }
            return new KeyValuePair<string,object>(pathPiece, h);

        }
    }


    [TestFixture]
    public class DirectoryPathTree_Specs
    {
        private DirectoryPathTree Tree;

        [SetUp]
        public void SetUpContext()
        {
            Tree = new DirectoryPathTree("C:\\foo");
        }

        [Test]
        public void Can_put_and_get()
        {
            Tree.PutRelative("C:\\foo\\bar\\baz", "foo\\bar\\baz");

            Tree.Get(new[] {"bar", "baz"}).ShouldBe("foo\\bar\\baz");
        }
    } 
}