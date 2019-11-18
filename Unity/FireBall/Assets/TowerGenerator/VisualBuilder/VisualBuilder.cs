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
        private TreeNode<Blueprint.Segment> _tree;
        public Transform Pivot;
        private readonly Vector3 ConnectorMargin = Vector3.one;

        public void Begin(TreeNode<Blueprint.Segment> tree)
        {
            Assert.IsNotNull(tree);
            _tree = tree;
        }

        readonly float[] _angles = { 0f, 90f, 180f, 270f };

        public void Step(TopologyGeneratorBase.TopGenStep cmd)
        {
            if (cmd.VisCmd == TopologyGeneratorBase.TopGenStep.VisualizationCmd.SegSpawn)
            {
                MetaBase meta = MetaProvider.Instance.Metas[0];
                Assert.IsNotNull(meta);
                var visSegPrefab = Resources.Load("Ents/" + meta.EntName);
                var visSegment = (GameObject)Instantiate(visSegPrefab);
                visSegment.name = visSegPrefab.name;

                visSegment.transform.position = Pivot.position + cmd.Segment.Data.Topology.Position;
                visSegment.transform.SetParent(Pivot);

                // calculate BB for vis segment
                Vector3 MaxBB = cmd.Segment.Data.Topology.AspectRatio - ConnectorMargin * 2;

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
            else if (cmd.VisCmd == TopologyGeneratorBase.TopGenStep.VisualizationCmd.SegDestroy)
            {
                //VisualizationSegment visSeg;
                //Assert.IsTrue(cmd.Segment.Data.Topology.IsOpenedForGenerator == false);
                //_visNodes.TryGetValue(cmd.Segment, out visSeg);
                //_stats.SegmentsAmount--;
                //Assert.IsNotNull(visSeg);
                //if (visSeg != null)
                //{
                //    visSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay * 0.5f).OnComplete(() => Destroy(visSeg.Chunk));
                //    _visNodes[cmd.Segment] = null;
                //}
            }
            else if (cmd.VisCmd == TopologyGeneratorBase.TopGenStep.VisualizationCmd.SegChangeState)
            {
                //VisualizationSegment visSeg;
                //var gotSeg = _visNodes.TryGetValue(cmd.Segment, out visSeg);
                //Assert.IsTrue(gotSeg, $"can't get {cmd.Segment} while trying to change");
                //Assert.IsNotNull(visSeg);
                //if (visSeg.SegType != cmd.Segment.Data.Topology.SegType)
                //{
                //    var newVisSeg = BuildTopologyVis(cmd.Segment); // spawn new one segment and marker(if needed)
                //    newVisSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay).From(); // appear animation
                //    _visNodes[cmd.Segment] = newVisSeg;
                //    visSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay * 0.5f).OnComplete(() => Destroy(visSeg.Chunk)); // disappear and destroy seg
                //}
                //else // only marker
                //{
                //    if (cmd.Segment.Data.Topology.IsOpenedForGenerator) // spawn marker
                //    {
                //        Assert.IsNull(visSeg.OpenedMarker);
                //        visSeg.OpenedMarker = Instantiate(prefabSegOpenedForGen);
                //        visSeg.OpenedMarker.name = "Opened";
                //        visSeg.OpenedMarker.transform.SetParent(visSeg.Chunk.transform);
                //        visSeg.OpenedMarker.transform.SetAsFirstSibling();
                //        visSeg.OpenedMarker.transform.DOScale(Vector3.zero, StepDelay).From(); // appear animation
                //    }
                //    else // despawn marker
                //    {
                //        Assert.IsNotNull(visSeg.OpenedMarker);
                //        visSeg.OpenedMarker.transform.DOScale(Vector3.zero, StepDelay * 0.5f).OnComplete(() => Destroy(visSeg.OpenedMarker));
                //    }
                //}
            }
        }
    }

}
