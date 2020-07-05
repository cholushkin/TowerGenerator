using System;
using System.Collections.Generic;
using System.Linq;
using GameLib;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class SegmentArchitect
    {
        public delegate Vector3 ChangeDirectionCallback(int index);

        // if we want:
        //  * dynamically created PlacementConfig (for example decreasing size every segment )
        //  * get some specifications ("very_big_peak,hat_like")
        public delegate GeneratorConfigBase.PlacementConfig GetPlacementConfigCallback(TopologyType topologyType, int segmentIndex);

        public ChangeDirectionCallback DirectionChanger { get; set; }
        public GetPlacementConfigCallback PlacementConfigProvider { get; set; }

        private RandomHelper _rnd;
        private long _initialSeed;
        private TreeNode<Blueprint.Segment> _treeCheck;
        private List<TreeNode<Blueprint.Segment>> _varLeafPointers;
        private TreeNode<Blueprint.Segment> _fromNode;
        private TreeNode<Blueprint.Segment> _startingNode;
        private GeneratorConfigBase _baseConfig;
        private TopologyType _beginningTopology;
        private TopologyType _middleTopology;
        private TopologyType _endingTopology;


        public SegmentArchitect(
            long seed,
            TreeNode<Blueprint.Segment> treeCheck, // from which part of the tree Architect should check collisions
            GeneratorConfigBase baseCfg,
            TopologyType beginningType, TopologyType middleType, TopologyType endingType,
            ChangeDirectionCallback directionChanger = null,
            GetPlacementConfigCallback placementConfigProvider = null
        )
        {
            _initialSeed = seed;
            _treeCheck = treeCheck;

            Assert.IsNotNull(baseCfg);
            _baseConfig = baseCfg;

            _beginningTopology = beginningType;
            _middleTopology = middleType;
            _endingTopology = endingType;

            DirectionChanger = directionChanger;
            PlacementConfigProvider = placementConfigProvider;
        }

        public bool MakeProjects(
            TreeNode<Blueprint.Segment> from, Range segCount,
            Vector3 offset, Vector3 direction
        )
        {
            if (_varLeafPointers != null)
            {
                Debug.Log("Reusing SegmentArchitect");
            }
            int targetSegmentCount = (int)segCount.From;
            int segmentCounter = 0;
            _varLeafPointers = new List<TreeNode<Blueprint.Segment>>((int)(segCount.To - segCount.From));
            _fromNode = from;
            TreeNode<Blueprint.Segment> nodePointer = from;

            _rnd = new RandomHelper(_initialSeed);

            while (segmentCounter != targetSegmentCount)
            {
                // get actual placement config
                var topologyType = (segmentCounter == 0) ? _beginningTopology : _middleTopology;
                if (segmentCounter == targetSegmentCount - 1 && (segmentCounter != 0))
                    topologyType = _endingTopology;
                GeneratorConfigBase.PlacementConfig placementConfig = null;
                if (PlacementConfigProvider != null)
                    placementConfig = PlacementConfigProvider(topologyType, segmentCounter);
                else
                    placementConfig = _baseConfig.GetPlacementConfig(topologyType);
                Assert.IsNotNull(placementConfig, "Can't find placement config");

                // create memory segment
                var memSeg = CreateMemorySegment(nodePointer, direction,
                    segmentCounter == 0 ? offset : Vector3.zero,
                    placementConfig);
                Assert.IsNotNull(memSeg);

                if (_startingNode == null)
                    _startingNode = memSeg;

                if (memSeg.Data.Topology.HasCollision)
                    break;

                if (segmentCounter == targetSegmentCount - 1)
                {
                    _varLeafPointers.Add(memSeg);
                    ++targetSegmentCount;
                    if (targetSegmentCount > segCount.To)
                        break;
                    continue;
                }
                ++segmentCounter;
                nodePointer = memSeg;
            }
            return GetProjectVariantsNumber() > 0;
        }


        public int GetProjectVariantsNumber()
        {
            if (_varLeafPointers == null)
                return 0;
            return _varLeafPointers.Count;
        }

        public TreeNode<Blueprint.Segment> GetProject(int index, out TreeNode<Blueprint.Segment> lastNode)
        {
            var choosen = _varLeafPointers[index];
            TreeNode<Blueprint.Segment> prevMemSegment = null;
            lastNode = null;
            foreach (var node in TreeNode<Blueprint.Segment>.TraverseToParent(null, choosen))
            {
                TreeNode<Blueprint.Segment> memSeg = new TreeNode<Blueprint.Segment>(node.Data);

                if (lastNode == null)
                    lastNode = memSeg;

                if (prevMemSegment != null)
                    memSeg.AddChild(prevMemSegment);

                prevMemSegment = memSeg;
            }
            return prevMemSegment;
        }

        private TreeNode<Blueprint.Segment> CreateMemorySegment(
            TreeNode<Blueprint.Segment> parentNode,
            Vector3 buildDirection,
            Vector3 offsetFromParent,
            GeneratorConfigBase.PlacementConfig placementConfig)
        {
            var memSeg = new Blueprint.Segment();
            var curNode = new TreeNode<Blueprint.Segment>(memSeg);
            if (parentNode != _fromNode)
                parentNode.AddChild(curNode);

            // get random meta from available meta providers of current placementConfig
            IEnumerable<MetaBase> allMetas = placementConfig.MetaProviders[0].GetMetas(); // available for current placement config
            for (int i = 1; i < placementConfig.MetaProviders.Length; ++i)
                allMetas = Enumerable.Concat(allMetas, placementConfig.MetaProviders[i].GetMetas());

            var meta = _rnd.FromEnumerable(
                MetaProvider.GetMetas(allMetas, placementConfig.MetaFilter));

            // exclude aabbs bigger than placementConfig.MetaFilter.BreadthRange and placementConfig.MetaFilter.HeightRange
            var aabbs = meta.AABBs.Where(a => MetaProvider.Filter.IsAABBInside(a,
                placementConfig.MetaFilter.BreadthRange, placementConfig.MetaFilter.HeightRange)).ToArray();

            Assert.IsTrue(aabbs.Length > 0);

            // get random aabb
            var aabb = _rnd.FromArray(aabbs);
            int sizeIndex = meta.AABBs.FindIndex(x => x == aabb);
            _rnd.FromList(meta.AABBs);

            Bounds bounds;
            if (parentNode != null)
            {
                var parentBounds = parentNode.Data.Topology.Geometry.Bounds;
                bounds = CreateBoundsForChild(parentBounds, buildDirection, meta.AABBs[sizeIndex], offsetFromParent);
            }
            else // parentNode is a proxy empty (establishment) node
            {
                bounds = CreateBounds(meta.AABBs[sizeIndex], offsetFromParent);
            }

            var hasCollision = CheckTreeCollisions(bounds, _treeCheck);
            hasCollision = hasCollision && CheckProjectCollisions(bounds, parentNode);

            memSeg.Topology = new Blueprint.Segment.TopologySegment
            {
                Geometry = new Blueprint.Segment.TopologySegment.ChunkGeometry { Bounds = bounds, BuildDirection = buildDirection, Meta = meta.name, Seed = _rnd.ValueInt(), SizeIndex = sizeIndex, TopologyType = meta.TopologyType },
                HasCollision = hasCollision,
                Connection = -buildDirection,
            };

            return curNode;
        }


        public static Bounds CreateBoundsForChild(Bounds parentBounds, Vector3 side, Vector3 childSize, Vector3 offset, float childScale = 1f)
        {
            var newBounds = new Bounds(parentBounds.center, childSize * childScale);
            newBounds.center += new Vector3(
                (parentBounds.size.x + newBounds.size.x) * 0.5f * side.x,
                (parentBounds.size.y + newBounds.size.y) * 0.5f * side.y,
                (parentBounds.size.z + newBounds.size.z) * 0.5f * side.z
            );
            newBounds.center += offset;
            return newBounds;
        }

        public static Bounds CreateBounds(Vector3 size, Vector3 offset, float scale = 1f)
        {
            var newBounds = new Bounds(Vector3.zero, size * scale);
            newBounds.center += offset;
            return newBounds;
        }

        public bool CheckTreeCollisions(Bounds checkBounds, TreeNode<Blueprint.Segment> tree)
        {
            if (tree == null)
                return false;
            foreach (var node in tree.TraverseDepthFirstPreOrder())
            {
                var nodeBounds = node.Data.Topology.Geometry.Bounds;
                if (nodeBounds.IntersectsEx(checkBounds))
                    return true;
            }
            return false;
        }

        // todo: from the start node of the project for each node check with checkBounds
        private bool CheckProjectCollisions(Bounds checkBounds, TreeNode<Blueprint.Segment> lastParent)
        {
            //if (lastParent == null)
            //    return false;

            //var pointer = lastParent;
            //while (pointer != _startingNode) 
            //{
            //    var pointerBounds = pointer.Data.ChunkGeometry.Bounds;
            //    if (pointerBounds.IntersectsEx(checkBounds))
            //        return true;
            //}
            return false;
        }

    }






    public static class BoundExtension
    {
        public static bool IntersectsEx(this Bounds b, Bounds bounds)
        {
            var hasCollision =
                (double)b.min.x < (double)bounds.max.x &&
                (double)b.max.x > (double)bounds.min.x &&
                (double)b.min.y < (double)bounds.max.y &&
                (double)b.max.y > (double)bounds.min.y &&
                (double)b.min.z < (double)bounds.max.z &&
                (double)b.max.z > (double)bounds.min.z;
            if (hasCollision)
            {
                var delta = (b.center - bounds.center);
                var ix = Mathf.Abs(delta.x) - (b.size.x + bounds.size.x) * 0.5f;
                var iy = Mathf.Abs(delta.y) - (b.size.y + bounds.size.y) * 0.5f;
                var iz = Mathf.Abs(delta.z) - (b.size.z + bounds.size.z) * 0.5f;

                if (ix >= 0f) // has no intrusion by x component
                    ix = 0f;
                if (iy >= 0f)
                    iy = 0f;
                if (iz >= 0f)
                    iz = 0f;

                double TOLERANCE = 0.0001f;
                if (Math.Abs(ix) > TOLERANCE && Math.Abs(iy) > TOLERANCE && Math.Abs(iz) > TOLERANCE)
                    return true;
            }
            return false;
        }
    }
}