using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class TreeNode<T>
    {
        public T Data { get; set; }

        public TreeNode<T> Parent { get; protected set; }

        public List<TreeNode<T>> Children { get; }

        public bool IsRoot => Parent == null;

        public bool IsLeaf => Children.Count == 0;

        public bool IsTrunk => BranchLevel == 0;

        public bool IsBranch => BranchLevel > 0;

        public int Level
        {
            get
            {
                if (IsRoot)
                    return 0;
                return Parent.Level + 1;
            }
        }

        public int BranchLevel
        {
            get
            {
                if (IsRoot)
                    return 0;
                return Parent.Children.First() == this ? Parent.BranchLevel : Parent.BranchLevel + 1;
            }
        }

        public TreeNode(T data)
        {
            Parent = null;
            Data = data;
            Children = new List<TreeNode<T>>();
        }

        public static void SwapWith(LinkedListNode<T> first, LinkedListNode<T> second)
        {
            Assert.IsNotNull(first);
            Assert.IsNotNull(second);

            var tmp = first.Value;
            first.Value = second.Value;
            second.Value = tmp;
        }

        public static void SwitchTrunk(TreeNode<T> node)
        {
            Assert.IsNotNull(node);
            if(node.BranchLevel == 0)
                return;

            var pointer = node;
            var prev = pointer;
            while (pointer.BranchLevel != 0)
            {
                prev = pointer;
                pointer = pointer.Parent;

                if (pointer.Children.Count > 1)
                {
                    var pIndex = pointer.Children.IndexOf(prev);
                    var tmp = pointer.Children[0];
                    pointer.Children[0] = pointer.Children[pIndex];
                    pointer.Children[pIndex] = tmp;
                }
            }

            Assert.IsTrue(pointer.BranchLevel == 0);
            Assert.IsTrue(pointer.Children.Count >= 2);
        }

        public void Disconnect()
        {
            foreach (var child in Children)
            {
                Assert.IsTrue(child.Parent == this);
                child.Parent = null;
            }
            Parent.Children.Remove(this);

            Parent = null;
            Children.Clear();
        }

        public TreeNode<T> AddChild(TreeNode<T> node)
        {
            Assert.IsNotNull(node);
            Children.Add(node);
            node.Parent = this;
            return node;
        }

        public TreeNode<T> AddChild(T data)
        {
            return AddChild(new TreeNode<T>(data));
        }

        public override string ToString()
        {
            var data = Data != null ? Data.ToString() : "[data null]";
            return $"Node:{GetHashCode()}:{data}";
        }

        public delegate TK DataMapper<TK>(T data);

        public static TreeNode<TK> Convert<TK>(TreeNode<T> tree, DataMapper<TK> mapper)
        {
            Stack<TreeNode<T>> nodeStack = new Stack<TreeNode<T>>();
            Stack<TreeNode<TK>> cloneStack = new Stack<TreeNode<TK>>();

            nodeStack.Push(tree);
            var cloneTree = new TreeNode<TK>(mapper(tree.Data));
            cloneStack.Push(cloneTree);

            while (nodeStack.Count != 0)
            {
                var currentNode = nodeStack.Pop();
                var curCloneNode = cloneStack.Pop();

                foreach (var treeNode in currentNode.Children)
                {
                    nodeStack.Push(treeNode);
                    var cloneChild = new TreeNode<TK>(mapper(treeNode.Data));
                    cloneStack.Push(cloneChild);
                    curCloneNode.AddChild(cloneChild);
                }
            }
            return cloneTree;
        }

        public static TreeNode<T> Clone(TreeNode<T> tree)
        {
            return Convert<T>(tree, d => d);
        }

        #region Tree traversing
        public IEnumerable<TreeNode<T>> TraverseDepthFirstPreOrder()
        {
            Stack<TreeNode<T>> nodeStack = new Stack<TreeNode<T>>();
            nodeStack.Push(this);

            while (nodeStack.Count != 0)
            {
                var currentNode = nodeStack.Pop();
                yield return currentNode;

                for (int i = currentNode.Children.Count - 1; i >= 0; i--)
                {
                    nodeStack.Push(currentNode.Children[i]);
                }
            }
        }

        public IEnumerable<TreeNode<T>> TraverseDepthFirstPostOrder()
        {
            Stack<TreeNode<T>> nodeStack = new Stack<TreeNode<T>>();
            nodeStack.Push(this);

            while (nodeStack.Count != 0)
            {
                var currentNode = nodeStack.Pop();
                yield return currentNode;

                foreach (var treeNode in currentNode.Children)
                    nodeStack.Push(treeNode);
            }
        }


        public IEnumerable<TreeNode<T>> TraverseBreadthFirst()
        {
            Queue<TreeNode<T>> queue = new Queue<TreeNode<T>>();
            queue.Enqueue(this);

            while (queue.Count != 0)
            {
                var n = queue.Count;
                while (n > 0)
                {
                    var node = queue.Dequeue();
                    yield return node;

                    foreach (var nodeChild in node.Children)
                        queue.Enqueue(nodeChild);
                    n--;
                }
            }
        }

        // traverse from 'from' to 'parent' excluding parent
        public static IEnumerable<TreeNode<T>> TraverseToParent(TreeNode<T> parent, TreeNode<T> from)
        {
            var pointer = from;
            Assert.IsTrue(parent != pointer);
            while (pointer != parent)
            {
                yield return pointer;
                pointer = pointer.Parent;
            }
        }

        #endregion
    }
}