using System.Collections;
using GameLib.DataStructures;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class TopologyGeneratorsManifoldBase : MonoBehaviour
    {
        public PointerProcessor Pointers;


        public float MaxDistanceProgressToGenerator;
        public float MaxDistanceProgressToGarabageCollector;

        public class PointerProcessor
        {
            public TreeNode<Blueprint.Segment> PointerGarbageCollector { get; private set; }
            public TreeNode<Blueprint.Segment> PointerProgress { get; private set; }
            public TreeNode<Blueprint.Segment> PointerGenerator { get; private set; }
            public TreeNode<Blueprint.Segment> PointerStable { get; set; }
            public float MaxDistanceProgressToGenerator;
            public float MaxDistanceProgressToGarbageCollector;
            private Blueprint _bp;

            public PointerProcessor(
                Blueprint bp,
                float maxDistanceProgressToGenerator,
                float maxDistanceProgressToGarbageCollector)
            {
                _bp = bp;
                MaxDistanceProgressToGenerator = maxDistanceProgressToGenerator;
                MaxDistanceProgressToGarbageCollector = maxDistanceProgressToGarbageCollector;
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
        protected Blueprint _bp;

        public virtual void StartGenerate(uint seed)
        {
            StartCoroutine(GenerateTopology(seed));
        }

        protected virtual IEnumerator GenerateTopology(uint seed)
        {
            return null;
        }

        protected virtual void FinalizeTower(uint seed)
        {

        }

        protected virtual void ResolveDeadlock()
        {

        }

        

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (_bp == null)
                return;
            if (_bp.Tree == null)
                return;
            //foreach (var treeNode in _bp.Tree.TraverseBreadthFirst())
            //{
            //    // node center
            //    Gizmos.color = Color.red;
            //    Gizmos.DrawSphere(
            //        transform.TransformPoint(treeNode.Data.Topology.Geometry.Position),
            //        0.5f);

            //    Gizmos.DrawWireCube(
            //        transform.TransformPoint(treeNode.Data.Topology.Geometry.Position), 
            //        treeNode.Data.Topology.Geometry.AspectRatio);

            //    Gizmos.color = Color.gray;
            //    Gizmos.DrawWireCube(
            //        transform.TransformPoint(treeNode.Data.Topology.Geometry.Position),
            //        treeNode.Data.Topology.Geometry.AspectRatio - TowerGeneratorConstants.ConnectorMargin);


            //    // all nodes children lines
            //    foreach (var child in treeNode.Children)
            //    {
            //        Gizmos.color = (child.BranchLevel == 0) ? Color.white : Color.grey;

            //        var childPos = transform.TransformPoint(child.Data.Topology.Geometry.Position);
            //        Gizmos.DrawLine(childPos, transform.TransformPoint(treeNode.Data.Topology.Geometry.Position));
            //    }
            //}

            //if (IsGizmoDrawPointers)
            //{
            //    // _pointerGenerator
            //    var pointerGeneratorPos = transform.TransformPoint(Pointers.PointerGenerator.Data.Topology.Geometry.Position);
            //    Gizmos.color = Color.black;
            //    Gizmos.DrawWireSphere(
            //        pointerGeneratorPos,
            //        1.0f);
            //    Handles.Label(pointerGeneratorPos, "PointerGenerator");


            //    // _pointerStable
            //    var pointerStablePos = transform.TransformPoint(Pointers.PointerStable.Data.Topology.Geometry.Position);
            //    Gizmos.color = Color.black;
            //    Gizmos.DrawWireSphere(
            //        pointerStablePos,
            //        1.0f);
            //    Handles.Label(pointerStablePos, "PointerStable");

            //    // _progressPointer
            //    var pointerProgress = transform.TransformPoint(Pointers.PointerProgress.Data.Topology.Geometry.Position);
            //    Gizmos.color = Color.white;
            //    Gizmos.DrawWireSphere(
            //        pointerProgress,
            //        1.0f);
            //    Handles.Label(pointerProgress, "PointerProgress");
            //    //Gizmos.DrawLine(pointerGeneratorPos, pointerProgress);

            //    // _pointerGarbageCollector
            //    var pointerGarbageCollectorPos =
            //        transform.TransformPoint(Pointers.PointerGarbageCollector.Data.Topology.Geometry.Position);
            //    Gizmos.color = Color.yellow;
            //    Gizmos.DrawWireSphere(
            //        pointerGarbageCollectorPos,
            //        1.0f);
            //    Handles.Label(pointerGarbageCollectorPos, "PointerGarbageCollector");
            //}
        }
#endif
    }
}
