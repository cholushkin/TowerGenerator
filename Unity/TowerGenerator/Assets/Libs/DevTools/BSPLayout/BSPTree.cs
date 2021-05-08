using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class BspTree
{
    public class Leaf
    {
        public enum Orientation
        {
            Vertical,
            Horizontal
        }

        public float Ratio;
        public Leaf LeafA;
        public Leaf LeafB;
        public Orientation LeafOrientation;

        // normalized global (coords system) values
        public Vector2 Min; // bottom left corner
        public Vector2 Size; // normalized width and height

        public Leaf(float ratio = 1f)
        {
            Ratio = ratio;
            LeafOrientation = Orientation.Horizontal;
            Min = Vector2.zero;
            Size = Vector2.one;
        }

        public Leaf SplitH(float ratio, out Leaf leafA, out Leaf leafB)
        {
            return LeafSplit(ratio, Orientation.Horizontal, out leafA, out leafB);
        }

        public Leaf SplitV(float ratio, out Leaf leafA, out Leaf leafB)
        {
            return LeafSplit(ratio, Orientation.Vertical, out leafA, out leafB);
        }

        private Leaf LeafSplit(float ratio, Orientation orientation, out Leaf leafA, out Leaf leafB)
        {
            ratio = Mathf.Clamp01(ratio);

            // leaf A
            {
                leafA = new Leaf(ratio);
                LeafA = leafA;
                LeafA.LeafOrientation = orientation;
                if (orientation == Orientation.Horizontal)
                {
                    leafA.Size.x = Size.x;
                    leafA.Size.y = Size.y * leafA.Ratio;
                    leafA.Min = Min;
                    leafA.Min.y += Size.y * (1f - ratio);
                }
                else // vertical
                {
                    leafA.Size.x = Size.x * leafA.Ratio;
                    leafA.Size.y = Size.y;
                    leafA.Min = Min;
                }
            }

            // leaf B
            {
                leafB = new Leaf(1f - ratio);
                LeafB = leafB;
                LeafB.LeafOrientation = orientation;
                if (orientation == Orientation.Horizontal)
                {
                    leafB.Size.x = Size.x;
                    leafB.Size.y = Size.y * leafB.Ratio;
                    leafB.Min = Min;
                }
                else // vertical
                {
                    leafB.Size.x = Size.x * leafB.Ratio;
                    leafB.Size.y = Size.y;
                    leafB.Min = Min;
                    leafB.Min.x += leafA.Size.x;
                }
            }
            return this;
        }
    }

    // post-order tree traversing using iterative alg
    public static IEnumerable<Leaf> GetTopLeafs(this Leaf tree)
    {
        Stack<Leaf> stack = new Stack<Leaf>(16);
        Leaf lastNodeVisited = null;
        var node = tree;

        while (stack.Count != 0 || node != null)
        {
            if (node != null)
            {
                stack.Push(node);
                node = node.LeafA;
            }
            else
            {
                var peekNode = stack.Peek();
                if (peekNode.LeafB != null && lastNodeVisited != peekNode.LeafB)
                {
                    Assert.IsNotNull(peekNode.LeafA);
                    node = peekNode.LeafB;
                }
                else
                {
                    var isTopLeaf = peekNode.LeafA == null && peekNode.LeafB == null;
                    if(isTopLeaf)
                        yield return peekNode;
                    lastNodeVisited = stack.Pop();
                }
            }
        }
    }
}
