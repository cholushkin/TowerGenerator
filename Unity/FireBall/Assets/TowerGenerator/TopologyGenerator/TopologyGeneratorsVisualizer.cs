﻿using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameLib.DataStructures;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public class TopologyGeneratorsVisualizer : MonoBehaviour
    {
        [Serializable]
        public class GeneratorStats
        {
            public int SegmentsAmount;
            public uint MaxHeight;
        }

        public class VisualizationSegment
        {
            public Blueprint.Segment.TopologySegment.ChunkType SegType;
            public GameObject Chunk;
            public GameObject OpenedMarker;
            public GameObject[] Connectors;
        }

        public GameObject prefabSegCollision;
        public GameObject prefabSegOpenedForGeneration;

        public GameObject prefabSegChunkRoofPeak;
        public GameObject prefabSegChunkStd;
        public GameObject prefabSegChunkIslandAndBasement;
        public GameObject prefabSegChunkSideEar;
        public GameObject prefabSegChunkBottomEar;
        public GameObject prefabSegChunkConnectorVertical;
        public GameObject prefabSegChunkConnectorHorizontal;

        public Transform Pivot;
        public float StepDelay;

        private TreeNode<Blueprint.Segment> _tree;
        private Dictionary<TreeNode<Blueprint.Segment>, VisualizationSegment> _visNodes;
        private GeneratorStats _stats;


        void OnGUI()
        {
            if (_stats != null)
            {
                GUI.Label(new Rect(0, 0, 200, 30), $"Segments: {_stats.SegmentsAmount}");
                GUI.Label(new Rect(0, 30, 200, 30), $"Height: {_stats.MaxHeight}");
            }
        }

        public WaitForSeconds Wait()
        {
            return new WaitForSeconds(StepDelay);
        }

        public void Begin(TreeNode<Blueprint.Segment> tree)
        {
            Assert.IsNotNull(tree);
            _tree = tree;
            _visNodes = new Dictionary<TreeNode<Blueprint.Segment>, VisualizationSegment>();
            _stats = new GeneratorStats();
        }

        public void Step(TopologyGeneratorBase.TopGenStep cmd)
        {
            if (cmd.VisCmd == TopologyGeneratorBase.TopGenStep.VisualizationCmd.SegSpawn)
            {
                var visSeg = BuildTopologyVis(cmd.Segment);
                visSeg.Chunk.transform.DOLocalMove(visSeg.Chunk.transform.localPosition + Vector3.up * 10, StepDelay)
                    .From();
                visSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay).From();
                _visNodes[cmd.Segment] = visSeg;

                _stats.SegmentsAmount++;
                if (_stats.MaxHeight < cmd.Segment.Data.Topology.Position.y)
                    _stats.MaxHeight = (uint)cmd.Segment.Data.Topology.Position.y;
            }
            else if (cmd.VisCmd == TopologyGeneratorBase.TopGenStep.VisualizationCmd.SegDestroy)
            {
                VisualizationSegment visSeg;
                Assert.IsTrue(cmd.Segment.Data.Topology.IsOpenedForGenerator == false);
                _visNodes.TryGetValue(cmd.Segment, out visSeg);
                _stats.SegmentsAmount--;
                Assert.IsNotNull(visSeg);
                if (visSeg != null)
                {
                    visSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay * 0.5f)
                        .OnComplete(() => Destroy(visSeg.Chunk));
                    _visNodes[cmd.Segment] = null;
                }
            }
            else if (cmd.VisCmd == TopologyGeneratorBase.TopGenStep.VisualizationCmd.SegChangeState)
            {
                VisualizationSegment visSeg;
                var gotSeg = _visNodes.TryGetValue(cmd.Segment, out visSeg);
                Assert.IsTrue(gotSeg, $"can't get {cmd.Segment} while trying to change");
                Assert.IsNotNull(visSeg);
                if (visSeg.SegType != cmd.Segment.Data.Topology.ChunkT)
                {
                    var newVisSeg = BuildTopologyVis(cmd.Segment); // spawn new one segment and marker(if needed)
                    newVisSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay).From(); // appear animation
                    _visNodes[cmd.Segment] = newVisSeg;
                    visSeg.Chunk.transform.DOScale(Vector3.zero, StepDelay * 0.5f)
                        .OnComplete(() => Destroy(visSeg.Chunk)); // disappear and destroy seg
                }
                else // only marker
                {
                    if (cmd.Segment.Data.Topology.IsOpenedForGenerator) // spawn marker
                    {
                        Assert.IsNull(visSeg.OpenedMarker);
                        visSeg.OpenedMarker = Instantiate(prefabSegOpenedForGeneration);
                        visSeg.OpenedMarker.name = "Opened";
                        visSeg.OpenedMarker.transform.SetParent(visSeg.Chunk.transform);
                        visSeg.OpenedMarker.transform.SetAsFirstSibling();
                        visSeg.OpenedMarker.transform.DOScale(Vector3.zero, StepDelay).From(); // appear animation
                    }
                    else // despawn marker
                    {
                        Assert.IsNotNull(visSeg.OpenedMarker);
                        visSeg.OpenedMarker.transform.DOScale(Vector3.zero, StepDelay * 0.5f)
                            .OnComplete(() => Destroy(visSeg.OpenedMarker));
                    }
                }
            }
        }

        public void End()
        {

        }

        VisualizationSegment BuildTopologyVis(TreeNode<Blueprint.Segment> node)
        {
            GameObject segment = null;

            var segTopology = node.Data.Topology;
            var visSeg = new VisualizationSegment();

            // create segment
            if (segTopology.HasCollision)
                segment = Instantiate(prefabSegCollision);
            else if (segTopology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkRoofPeak)
                segment = Instantiate(prefabSegChunkRoofPeak);
            else if (segTopology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkStd)
                segment = Instantiate(prefabSegChunkStd);
            else if (segTopology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkIslandAndBasement)
                segment = Instantiate(prefabSegChunkIslandAndBasement);
            else if (segTopology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkSideEar)
                segment = Instantiate(prefabSegChunkSideEar);
            else if (segTopology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkBottomEar)
                segment = Instantiate(prefabSegChunkBottomEar);
            else if (segTopology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkConnectorVertical)
                segment = Instantiate(prefabSegChunkConnectorVertical);
            else if (segTopology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkConnectorHorizontal)
                segment = Instantiate(prefabSegChunkConnectorHorizontal);

            GameObject opened = null;
            if (segTopology.IsOpenedForGenerator)
            {
                opened = Instantiate(prefabSegOpenedForGeneration);
                opened.name = "Opened";
            }

            // set pos & hierarchy
            segment.transform.position = _generatorPivot.position + segTopology.Position;
            segment.transform.localScale = segTopology.AspectRatio;
            segment.transform.SetParent(_generatorPivot);
            segment.name = $"{segment}";

            if (opened != null)
            {
                opened.transform.position = segment.transform.position;
                opened.transform.SetParent(segment.transform);
                opened.transform.SetAsFirstSibling();
            }

            visSeg.Chunk = segment;
            visSeg.OpenedMarker = opened;
            visSeg.SegType = segTopology.ChunkT;

            return visSeg;
        }

        private Transform _generatorPivot;
        public void ChangeGenerator(TopologyGeneratorBase curGenerator, uint generatorChainCounter)
        {
            var genObj = new GameObject(curGenerator.GetType().ToString().Split('.').Last() + "." + generatorChainCounter);
            genObj.transform.position = Pivot.position;
            genObj.transform.SetParent(Pivot);
            _generatorPivot = genObj.transform;
        }
    }
}