using System.Linq;
using Assets.Plugins.Alg;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class VisualBuilder : MonoBehaviour
    {
        private RandomHelper _rnd = new RandomHelper(-1); // todo:
        private TreeNode<Blueprint.Segment> _curPointer;
        public Transform Pivot;
        private readonly Vector3 ConnectorMargin = Vector3.one;

        public void Begin(TreeNode<Blueprint.Segment> tree)
        {
            Assert.IsNotNull(tree);
            _curPointer = tree;
        }

        readonly float[] _angles = { 0f, 90f, 180f, 270f };

        public void Build(TreeNode<Blueprint.Segment> dstNode)
        {
            foreach (var node in _curPointer.TraverseDepthFirstPreOrder())
            {
                BuildSegment(node);
                if (node == dstNode)
                {
                    _curPointer = dstNode;
                    return;
                }
            }
        }

        private void BuildSegment(TreeNode<Blueprint.Segment> node)
        {
            // get any random meta of appropriate chunk type
            MetaBase meta = MetaProvider.Instance.Metas.Select(x => x as MetaChunk)
                .FirstOrDefault(x => x.ChunkType == node.Data.Topology.ChunkT);
            var visSegPrefab = Resources.Load("Ents/" + meta.EntName);
            var visSegment = (GameObject) Instantiate(visSegPrefab);
            visSegment.name = visSegPrefab.name;

            visSegment.transform.position = Pivot.position + node.Data.Topology.Position;
            visSegment.transform.SetParent(Pivot);

            // calculate BB for vis segment
            Vector3 MaxBB = node.Data.Topology.AspectRatio - ConnectorMargin * 2;

            var visSegController = visSegment.GetComponent<GroupsController>();
            var isOK = visSegController.SetMaximizedFitRndConfiguration(MaxBB);
            if (!isOK)
                visSegment.name += "error:missfit";


            // rotation
            visSegment.transform.Rotate(visSegment.transform.up, _rnd.FromArray(_angles));

            // centering
            var offset = visSegment.transform.position - visSegment.BoundBox().center;
            visSegment.transform.position += offset;

                

                //var visSeg = BuildTopologyVis(cmd.Segment);
                //visSeg.Chunk.transform.DOLocalMove(visSeg.Chunk.transform.localPosition + Vector3.up * 10, StepDelay).From();
                //visSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay).From();
                //_visNodes[cmd.Segment] = visSeg;

                //_stats.SegmentsAmount++;
                //if (_stats.MaxHeight < cmd.Segment.Data.Topology.Position.y)
                //    _stats.MaxHeight = (uint)cmd.Segment.Data.Topology.Position.y;
        }
    }
}
