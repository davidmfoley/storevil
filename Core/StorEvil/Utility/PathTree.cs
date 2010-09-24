using System;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Utility
{
    public class PathTree
    {
        public PathTree()
        {
            Root = new PathTreeBranchNode();
        }

        public object Transform(Func<string, object[], object> buildBranch, Func<string, object, object> buildLeaf)
        {
            var transformer = new PathTreeTransformer(buildBranch, buildLeaf);
            return transformer.Transform("", Root);
        }

        public class PathTreeBranchNode : IPathTreeNode
        {
            private Dictionary<string, IPathTreeNode> _children = new Dictionary<string, IPathTreeNode>();

            public Dictionary<string, IPathTreeNode> Children
            {
                get { return _children; }
            }

            public object Item
            {
                get { return null; }
            }

            public void Put(string[] path, object item)
            {
                var first = path.First();

                if (path.Count() == 1)
                {
                    _children.Add(first, new PathTreeLeafNode(item));
                }
                else
                {
                    if (!_children.ContainsKey(first))
                        _children.Add(first, new PathTreeBranchNode());

                    _children[first].Put(path.Skip(1).ToArray(), item);
                }
            }

            public object Get(string[] path)
            {
                if (!path.Any())
                    return this;

                if (_children.ContainsKey(path.First()))
                    return _children[path.First()].Get(path.Skip(1).ToArray());

                return null;
            }
        }

        public interface IPathTreeNode
        {
            object Get(string[] path);
            void Put(string[] path, object item);

            Dictionary<string, IPathTreeNode> Children { get; }
            object Item { get; }
        }

        public class PathTreeLeafNode : IPathTreeNode
        {
            private readonly object _item;

            public object Get(string[] path)
            {
                if (path.Any())
                    return null;

                return _item;
            }

            public void Put(string[] path, object item)
            {
                throw new InvalidOperationException("Can't put a child in a leaf node.");
            }

            public Dictionary<string, IPathTreeNode> Children
            {
                get { return new Dictionary<string, IPathTreeNode>(); }
            }

            public object Item
            {
                get { return _item; }
            }

            public PathTreeLeafNode(object item)
            {
                _item = item;
            }
        }

        public IPathTreeNode Root;
        public int Count { get { return 0; } }

        public void Put(string[] path, object item)
        {
            Root.Put(path, item);
        }

        public object Get(string[] path)
        {
            return Root.Get(path);
        }
    }

    public class DirectoryPathTree : PathTree
    {
        private readonly string _root;

        public DirectoryPathTree(string root)
        {
            _root = root;
        }

        public void PutRelative(string path, object item)
        {
            var pieces = PathHelper.GetRelativePathPieces(_root, path);
            Put(pieces, item);
        }
    }

    public class PathTreeTransformer
    {
        private readonly Func<string, object[], object> _buildBranch;
        private readonly Func<string, object, object> _buildLeaf;

        public PathTreeTransformer(Func<string, object[], object> buildBranch, Func<string, object, object> buildLeaf)
        {
            _buildBranch = buildBranch;
            _buildLeaf = buildLeaf;
        }


        public object Transform(string path, PathTree.IPathTreeNode tree)
        {
            if (!(tree is PathTree.PathTreeBranchNode))
                return _buildLeaf(path, tree.Item);

            var children = tree.Children.Select(n => Transform(n.Key, n.Value)).ToArray();
            return _buildBranch(path, children);
        }
    }
}