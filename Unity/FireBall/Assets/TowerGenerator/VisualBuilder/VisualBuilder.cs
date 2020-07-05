//using System.Linq;
//using Assets.Plugins.Alg;
//using GameLib.DataStructures;
//using GameLib.Random;
//using UnityEngine;
//using UnityEngine.Assertions;

using UnityEngine;

namespace TowerGenerator
{
    public class VisualBuilder : MonoBehaviour
    {
        //        private RandomHelper _rnd = new RandomHelper(-1); // todo:
        //        private TreeNode<Blueprint.Segment> _curPointer;
        //        public Transform Pivot;
        //        private readonly Vector3 ConnectorMargin = Vector3.one;

        //        public void Begin(TreeNode<Blueprint.Segment> tree)
        //        {
        //            Assert.IsNotNull(tree);
        //            _curPointer = tree;
        //        }

        //        readonly float[] _angles = { 0f, 90f, 180f, 270f };

        //        public void Build(TreeNode<Blueprint.Segment> dstNode)
        //        {
        //            foreach (var node in _curPointer.TraverseDepthFirstPreOrder())
        //            {
        //                BuildSegment(node);
        //                if (node == dstNode)
        //                {
        //                    _curPointer = dstNode;
        //                    return;
        //                }
        //            }
        //        }

        //        private void BuildSegment(TreeNode<Blueprint.Segment> node)
        //        {
        //            // get any random meta of appropriate chunk type
        //            MetaBase meta = MetaProvider.Instance.Metas.Select(x => x as MetaChunk)
        //                .FirstOrDefault(x => x.TopologyType == node.Data.Topology.Geometry.TopologyType);
        //            var visSegPrefab = Resources.Load("Chunks/" + meta.ChunkName);
        //            var visSegment = (GameObject) Instantiate(visSegPrefab);
        //            visSegment.name = visSegPrefab.name;

        //            visSegment.transform.position = Pivot.position + node.Data.Topology.Geometry.Bounds.center;
        //            visSegment.transform.SetParent(Pivot);

        //            // calculate BB for vis segment
        //            Vector3 MaxBB = node.Data.Topology.Geometry.Bounds.size - ConnectorMargin;

        //            var visSegController = visSegment.GetComponent<RootGroupsController>();
        //            //var isOK = visSegController.SetMaximizedFitRndConfiguration(MaxBB);
        //            //if (!isOK)
        //            //    visSegment.name += "error:missfit";

        //            // rotation
        //            visSegment.transform.Rotate(visSegment.transform.up, _rnd.FromArray(_angles));

        //            // centering
        //            var segBB = visSegment.BoundBox();
        //            var offset = visSegment.transform.position - segBB.center;
        //            visSegment.transform.position += offset;
        //            segBB.center = visSegment.transform.position;

        //            // snapping
        //            if (node.Data.Topology.Geometry.TopologyType == TopologyType.ChunkPeak)
        //                segBB = SnapBBPos( Vector3.down, new Bounds(visSegment.transform.position, MaxBB), segBB);
        //            else if (node.Data.Topology.Geometry.TopologyType.HasFlag(TopologyType.ChunkFoundation))
        //                segBB = SnapBBPos(Vector3.up, new Bounds(visSegment.transform.position, MaxBB), segBB);

        //            visSegment.transform.position = segBB.center;

        //            //var visSeg = BuildTopologyVis(cmd.Segment);
        //            //visSeg.Chunk.transform.DOLocalMove(visSeg.Chunk.transform.localPosition + Vector3.up * 10, StepDelay).From();
        //            //visSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay).From();
        //            //_visNodes[cmd.Segment] = visSeg;

        //            //_stats.SegmentsAmount++;
        //            //if (_stats.MaxHeight < cmd.Segment.Data.Topology.Position.y)
        //            //    _stats.MaxHeight = (uint)cmd.Segment.Data.Topology.Position.y;
        //        }

        //        private Bounds SnapBBPos(Vector3 inDirection, Bounds outerBB, Bounds BB)
        //        {
        //            var deltaExtents = outerBB.extents - BB.extents;
        //            if (inDirection == Vector3.down)
        //                BB.center = new Vector3(BB.center.x, BB.center.y - deltaExtents.y, BB.center.z);
        //            if (inDirection == Vector3.up)
        //                BB.center = new Vector3(BB.center.x, BB.center.y + deltaExtents.y, BB.center.z);
        //            return BB;
        //        }
    }
}
