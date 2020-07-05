//using System;
//using System.Collections.Generic;
//using System.Linq;
//using DG.Tweening;
//using GameLib;
//using GameLib.DataStructures;
//using UnityEngine;
//using UnityEngine.Assertions;


//namespace TowerGenerator
//{
//    public class TopologyGeneratorsVisualizer : MonoBehaviour
//    {
//        // topology generation elementary step for visualization
//        public class TopGenStep
//        {
//            public enum Cmd
//            {
//                SegSpawn,
//                SegDestroy,
//                SegChangeState,
//            }

//            public TreeNode<Blueprint.Segment> Segment { get; }
//            public Cmd GeneratorCmd { get; }

//            private TopGenStep(TreeNode<Blueprint.Segment> segment, Cmd cmd)
//            {
//                Segment = segment;
//                GeneratorCmd = cmd;
//            }

//            public static TopGenStep DoStep(TreeNode<Blueprint.Segment> segment, Cmd cmd)
//            {
//                Assert.IsNotNull(segment);
//                return new TopGenStep(segment, cmd);
//            }
//        }

//        [Serializable]
//        public class GeneratorStats
//        {
//            public int SegmentsAmount;
//            public uint MaxHeight;
//        }

//        public class VisualizationSegment
//        {
//            public TopologyType TopologyType;
//            public GameObject Chunk;
//            public GameObject OpenedMarker;
//            public GameObject[] Connectors;
//        }

//        public GameObject prefabSegCollision;
//        public GameObject prefabSegOpenedForGeneration;

//        public GameObject prefabSegChunkRoofPeak;
//        public GameObject prefabSegChunkStd;
//        public GameObject prefabSegChunkIslandAndBasement;
//        public GameObject prefabSegChunkSideEar;
//        public GameObject prefabSegChunkBottomEar;
//        public GameObject prefabSegChunkConnectorVertical;
//        public GameObject prefabSegChunkConnectorHorizontal;

//        public Transform Pivot;
//        public float StepDelay;

//        private TreeNode<Blueprint.Segment> _tree;
//        private Dictionary<TreeNode<Blueprint.Segment>, VisualizationSegment> _visNodes;
//        private GeneratorStats _stats;


//        void OnGUI()
//        {
//            if (_stats != null)
//            {
//                GUI.Label(new Rect(0, 0, 200, 30), $"Segments: {_stats.SegmentsAmount}");
//                GUI.Label(new Rect(0, 30, 200, 30), $"Height: {_stats.MaxHeight}");
//            }
//        }

//        public WaitForSeconds Wait()
//        {
//            return new WaitForSeconds(StepDelay);
//        }

//        public void Begin(TreeNode<Blueprint.Segment> tree)
//        {
//            Assert.IsNotNull(tree);
//            _tree = tree;
//            _visNodes = new Dictionary<TreeNode<Blueprint.Segment>, VisualizationSegment>();
//            _stats = new GeneratorStats();
//        }

//        public void Step(TopGenStep step)
//        {
//            if (step.GeneratorCmd == TopGenStep.Cmd.SegSpawn)
//            {
//                var visSeg = BuildTopologyVis(step.Segment);
//                visSeg.Chunk.transform.DOLocalMove(visSeg.Chunk.transform.localPosition + Vector3.up * 10, StepDelay)
//                    .From();
//                visSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay).From();
//                _visNodes[step.Segment] = visSeg;

//                _stats.SegmentsAmount++;
//                if (_stats.MaxHeight < step.Segment.Data.Topology.Geometry.Bounds.center.y)
//                    _stats.MaxHeight = (uint)step.Segment.Data.Topology.Geometry.Bounds.center.y;
//            }
//            else if (step.GeneratorCmd == TopGenStep.Cmd.SegDestroy)
//            {
//                VisualizationSegment visSeg;
//                Assert.IsTrue(step.Segment.Data.Topology.IsOpenedForGenerator == false);
//                _visNodes.TryGetValue(step.Segment, out visSeg);
//                _stats.SegmentsAmount--;
//                Assert.IsNotNull(visSeg);
//                if (visSeg != null)
//                {
//                    visSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay * 0.5f)
//                        .OnComplete(() => Destroy(visSeg.Chunk));
//                    _visNodes[step.Segment] = null;
//                }
//            }
//            else if (step.GeneratorCmd == TopGenStep.Cmd.SegChangeState)
//            {
//                VisualizationSegment visSeg;
//                var gotSeg = _visNodes.TryGetValue(step.Segment, out visSeg);
//                Assert.IsTrue(gotSeg, $"can't get {step.Segment} while trying to change");
//                Assert.IsNotNull(visSeg);
//                if (visSeg.TopologyType != step.Segment.Data.Topology.Geometry.TopologyType)
//                {
//                    var newVisSeg = BuildTopologyVis(step.Segment); // spawn new one segment and marker(if needed)
//                    newVisSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay).From(); // appear animation
//                    _visNodes[step.Segment] = newVisSeg;
//                    visSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay * 0.5f)
//                        .OnComplete(() => Destroy(visSeg.Chunk)); // disappear and destroy seg
//                }
//                else // only marker
//                {
//                    if (step.Segment.Data.Topology.IsOpenedForGenerator) // spawn marker
//                    {
//                        Assert.IsNull(visSeg.OpenedMarker);
//                        visSeg.OpenedMarker = Instantiate(prefabSegOpenedForGeneration);
//                        visSeg.OpenedMarker.name = "Opened";
//                        visSeg.OpenedMarker.transform.SetParent(visSeg.Chunk.transform);
//                        visSeg.OpenedMarker.transform.SetAsFirstSibling();
//                        visSeg.OpenedMarker.transform.DOScale(Vector3.zero, StepDelay).From(); // appear animation
//                    }
//                    else // despawn marker
//                    {
//                        Assert.IsNotNull(visSeg.OpenedMarker);
//                        visSeg.OpenedMarker.transform.DOScale(Vector3.zero, StepDelay * 0.5f)
//                            .OnComplete(() => Destroy(visSeg.OpenedMarker));
//                    }
//                }
//            }
//        }

//        public void End()
//        {

//        }

//        VisualizationSegment BuildTopologyVis(TreeNode<Blueprint.Segment> node)
//        {
//            GameObject segment = null;

//            var segTopology = node.Data.Topology;
//            var visSeg = new VisualizationSegment();

//            // create segment
//            if (segTopology.HasCollision)
//                segment = Instantiate(prefabSegCollision);
//            else if (segTopology.Geometry.TopologyType == TopologyType.ChunkPeak)
//                segment = Instantiate(prefabSegChunkRoofPeak);
//            else if (segTopology.Geometry.TopologyType == TopologyType.ChunkStd)
//                segment = Instantiate(prefabSegChunkStd);
//            else if (segTopology.Geometry.TopologyType.HasFlag(TopologyType.ChunkFoundation))
//                segment = Instantiate(prefabSegChunkIslandAndBasement);
//            else if (segTopology.Geometry.TopologyType == TopologyType.ChunkSideEar)
//                segment = Instantiate(prefabSegChunkSideEar);
//            else if (segTopology.Geometry.TopologyType == TopologyType.ChunkBottomEar)
//                segment = Instantiate(prefabSegChunkBottomEar);

//            // create connector
//            GameObject connector = null;
//            if (node.Data.Topology.Connection != Vector3.zero)
//            {
//                if (Direction.IsVertical(node.Data.Topology.Connection))
//                    connector = Instantiate(prefabSegChunkConnectorVertical);
//                else
//                    connector = Instantiate(prefabSegChunkConnectorHorizontal);
//            }

//            GameObject opened = null;
//            if (segTopology.IsOpenedForGenerator)
//            {
//                opened = Instantiate(prefabSegOpenedForGeneration);
//                opened.name = "Opened";
//            }

//            // set pos & hierarchy
//            segment.transform.position = _generatorPivot.position + segTopology.Geometry.Bounds.center;
//            segment.transform.localScale = segTopology.Geometry.Bounds.size;
//            segment.transform.SetParent(_generatorPivot);
//            segment.name = $"{segment}";

//            if (connector != null)
//            {
//                connector.transform.position = segment.transform.position;
//                var m = Direction.IsVertical(node.Data.Topology.Connection)
//                    ? segTopology.Geometry.Bounds.size.y * 0.5f
//                    : segTopology.Geometry.Bounds.size.x * 0.5f;
//                connector.transform.position += node.Data.Topology.Connection * m;

//                connector.transform.localScale = Direction.IsVertical(node.Data.Topology.Connection)
//                    ? new Vector3(
//                        connector.transform.localScale.x * TowerGeneratorConstants.ConnectorAspect,
//                        connector.transform.localScale.y,
//                        connector.transform.localScale.z * TowerGeneratorConstants.ConnectorAspect)
//                    : new Vector3(
//                        connector.transform.localScale.x * TowerGeneratorConstants.ConnectorAspect,
//                        connector.transform.localScale.y * TowerGeneratorConstants.ConnectorAspect,
//                        connector.transform.localScale.z);
//                connector.transform.SetParent(segment.transform);
//            }

//            if (opened != null)
//            {
//                opened.transform.position = segment.transform.position;
//                opened.transform.SetParent(segment.transform);
//                opened.transform.SetAsFirstSibling();
//            }

//            visSeg.Chunk = segment;
//            visSeg.OpenedMarker = opened;
//            visSeg.TopologyType = segTopology.Geometry.TopologyType;

//            return visSeg;
//        }

//        private Transform _generatorPivot;
//        private uint _generatorChainCounter;
//        public void ChangeGenerator(GeneratorBase curGenerator)
//        {
//            var genObj = new GameObject(curGenerator.GetType().ToString().Split('.').Last() + "." + _generatorChainCounter);
//            genObj.transform.position = Pivot.position;
//            genObj.transform.SetParent(Pivot);
//            _generatorPivot = genObj.transform;
//            ++_generatorChainCounter;
//        }
//    }
//}