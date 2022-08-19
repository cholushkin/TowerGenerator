using GameLib.DataStructures;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class GeneratorPointer
    {
        public TreeNode<Blueprint.Segment> PointerGeneratorTopTrunk { get; private set; } // everything below PointerGarbageCollector could be collected
        public TreeNode<Blueprint.Segment> PointerGeneratorStable { get; private set; } // last stable node (also last collision check pivot). Constructor never goes above the stable node because a tree there could be regenerated
        public TreeNode<Blueprint.Segment> PointerGarbageCollectorAbove { get; private set; } // everything above PointerGarbageCollectorAbove could be collected (excluding PointerGarbageCollectorAbove pointer itself)
        public TreeNode<Blueprint.Segment> PointerGarbageCollectorBelow { get; private set; } // everything below PointerGarbageCollectorBelow could be collected (excluding PointerGarbageCollectorBelow pointer itself)

        public TreeNode<Blueprint.Segment> PointerConstructFrom { get; private set; } // constructor builds from this pointer (including)
        public TreeNode<Blueprint.Segment> PointerConstructTo { get; private set; } // constructor builds to this pointer (including)
        public TreeNode<Blueprint.Segment> PointerViewport { get; private set; }

        public bool IsNeededToGenerateMore => DistanceDown(PointerGeneratorTopTrunk, PointerViewport) < ViewportToTopTrunkDistance;

        private const int ViewportToTopTrunkDistance = 32; // how many segments from PointerViewport up to PointerGeneratorTopTrunk needs to be to pause generation
        private int GarbageCollectorDistance = 64; // how many segments from PointerViewport down&up to GarbageCollector pointers

        private Blueprint _bp;

        public GeneratorPointer(Blueprint bp)
        {
            _bp = bp;
            Assert.IsTrue(GarbageCollectorDistance > ViewportToTopTrunkDistance);
        }

        public void SetInitialPointers()
        {
            Assert.IsNotNull(_bp);
            Assert.IsNotNull(_bp.Tree);
            PointerGarbageCollectorAbove = PointerGarbageCollectorBelow = PointerGeneratorTopTrunk = PointerGeneratorStable
            = PointerConstructFrom = PointerConstructTo = PointerViewport = _bp.Tree;
        }

        public void SetPointerGeneratorTopTrunk(TreeNode<Blueprint.Segment> pointerGeneratorTopTrunk, TreeNode<Blueprint.Segment> pointerGeneratorStable)
        {
            Assert.IsNotNull(pointerGeneratorTopTrunk);
            Assert.IsNotNull(pointerGeneratorStable);
            PointerGeneratorTopTrunk = pointerGeneratorTopTrunk;
            PointerGeneratorStable = pointerGeneratorStable;

            // refresh pointer viewport
            SetPointerViewport(PointerViewport);
        }

        public void SetPointerViewport(TreeNode<Blueprint.Segment> pointerViewport)
        {
            Assert.IsNotNull(pointerViewport);
            PointerViewport = pointerViewport;

            // refresh construct pointers
            PointerConstructFrom = GetBelow(PointerViewport, ViewportToTopTrunkDistance);
            PointerConstructTo = GetAbove(PointerViewport, ViewportToTopTrunkDistance);

            // refresh garbage collector pointers
            PointerGarbageCollectorBelow = GetBelow(pointerViewport, GarbageCollectorDistance);
            PointerGarbageCollectorAbove = GetAbove(pointerViewport, GarbageCollectorDistance);
        }

        private TreeNode<Blueprint.Segment> GetBelow(TreeNode<Blueprint.Segment> from, int distance)
        {
            int counter = 0;
            var pointer = from;
            while (pointer != null)
            {
                if (counter == distance)
                    return pointer;
                counter++;
                pointer = pointer.Parent;
            }
            return _bp.Tree;
        }

        public static TreeNode<Blueprint.Segment> GetAbove(TreeNode<Blueprint.Segment> from, int distance)
        {
            int counter = 0;
            var pointer = from;
            while (pointer != null)
            {
                if (counter == distance)
                    return pointer;

                counter++;

                if (pointer.Children.Count == 0)
                    return pointer;

                pointer = pointer.Children[0];
            }
            return null;
        }

        public int DistanceUp(TreeNode<Blueprint.Segment> from, TreeNode<Blueprint.Segment> to)
        {
            int counter = 0;
            var pointer = from;
            while (pointer != to && pointer.Children.Count != 0)
            {
                counter++;
                pointer = pointer.Children[0];
            }
            return counter;
        }


        public int DistanceDown(TreeNode<Blueprint.Segment> from, TreeNode<Blueprint.Segment> to)
        {
            int counter = 0;
            var pointer = from;
            while (pointer != to && pointer != null)
            {
                counter++;
                pointer = pointer.Parent;
            }
            return counter;
        }
    }
}