using GameLib.DataStructures;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class GeneratorPointer 
    {
        public TreeNode<Blueprint.Segment> PointerGarbageCollector { get; private set; }
        public TreeNode<Blueprint.Segment> PointerProgress { get; private set; }
        public TreeNode<Blueprint.Segment> PointerGenerator { get; private set; }
        public TreeNode<Blueprint.Segment> PointerStable { get; set; }
        public float MaxDistanceProgressToGenerator;
        public float MaxDistanceProgressToGarbageCollector;
        private Blueprint _bp;

        public GeneratorPointer(
            Blueprint bp
            //float maxDistanceProgressToGenerator,
            //float maxDistanceProgressToGarbageCollector
            )
        {
            _bp = bp;
            //MaxDistanceProgressToGenerator = maxDistanceProgressToGenerator;
            //MaxDistanceProgressToGarbageCollector = maxDistanceProgressToGarbageCollector;
        }

        public void SetInitialPointers()
        {
            Assert.IsNotNull(_bp);
            Assert.IsNotNull(_bp.Tree);
            PointerGarbageCollector = _bp.Tree;
            PointerProgress = _bp.Tree;
            PointerGenerator = _bp.Tree;
            PointerStable = _bp.Tree;
        }

        //public float DistanceYFactorProgress2Generator()
        //{
        //    return Mathf.Abs(PointerProgress.Data.Topology.Geometry.Position.y -
        //                     PointerGenerator.Data.Topology.Geometry.Position.y);
        //}

        //public void MoveProgress()
        //{
        //    var nextTrunkNode = PointerProgress.Children.FirstOrDefault();
        //    if (nextTrunkNode == null)
        //        return;

        //    // Update progress pointer
        //    PointerProgress = nextTrunkNode;

        //    // Decrease distance between PointerProgress and PointerGarbageCollector
        //    while (Mathf.Abs(PointerProgress.Data.Topology.Geometry.Position.y - PointerGarbageCollector.Data.Topology.Geometry.Position.y)
        //           > MaxDistanceProgressToGarbageCollector) // y distance from PointerProgress to PointerGarbageCollector
        //    {
        //        MoveGarbageCollectorPointer();
        //    }
        //}

        //private void MoveGarbageCollectorPointer()
        //{
        //    PointerGarbageCollector = PointerGarbageCollector.Children.First();
        //}
    }



}
