using System;
using System.Collections.Generic;
using System.Linq;
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
        //  * dynamically created PlacementConfig (decreasing size every segment for example)
        //  * get some specifications ("very_big_peak,hat_like")
        public delegate GeneratorConfigBase.PlacementConfig GetPlacementConfigCallback(TopologyType topologyType, int segmentIndex);

        public class MemorySegment
        {
            public Blueprint.Segment.TopologySegment.ChunkGeometry ChunkGeometry;
            public bool HasCollision { get; set; }
        }

        public ChangeDirectionCallback DirectionChanger { get; set; }
        public GetPlacementConfigCallback PlacementConfigProvider { get; set; }


        private RandomHelper _rnd;
        private TreeNode<Blueprint.Segment> _blueprintTree;
        private List<TreeNode<MemorySegment>> _varLeafPointers;
        private TreeNode<MemorySegment> _startingNode;
        private GeneratorConfigBase _baseConfig;


        public SegmentArchitect(long seed, TreeNode<Blueprint.Segment> tree) 
        {
            _rnd = new RandomHelper(seed);
            _blueprintTree = tree;
        }

        public bool Project(
            GeneratorConfigBase baseCfg, Blueprint.Segment.TopologySegment.ChunkGeometry from, Range segCount,
            Vector3 direction, Vector3 offset,
            TopologyType beginningType, TopologyType middleType, TopologyType lastType
        )
        {
            Assert.IsTrue(_varLeafPointers == null || _varLeafPointers.Count == 0);
            Assert.IsNotNull(baseCfg);
            _baseConfig = baseCfg;
            int targetSegmentCount = (int)segCount.From;
            int segmentCounter = 0;
            _varLeafPointers = new List<TreeNode<MemorySegment>>((int)(segCount.To - segCount.From));
            TreeNode<MemorySegment> nodePointer = new TreeNode<MemorySegment>(new MemorySegment{ChunkGeometry = from});
            _startingNode = nodePointer;

            while (segmentCounter != targetSegmentCount)
            {
                // get actual placement config
                var topologyType = (segmentCounter == 0) ? beginningType : middleType;
                if (segmentCounter == targetSegmentCount - 1 && (segmentCounter != 0))
                    topologyType = lastType;
                GeneratorConfigBase.PlacementConfig placementConfig = null;
                if (PlacementConfigProvider != null)
                    placementConfig = PlacementConfigProvider(topologyType, segmentCounter);
                else
                    placementConfig = baseCfg.GetPlacementConfig(topologyType);
                Assert.IsNotNull(placementConfig, "Can't find placement config");

                // create memory segment
                var memSeg = CreateMemorySegment(nodePointer, direction,
                    segmentCounter == 0 ? offset : Vector3.zero,
                    placementConfig);

                if (memSeg.Data.HasCollision)
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

        public TreeNode<MemorySegment> GetProject(int index)
        {
            var choosen = _varLeafPointers[index];
            TreeNode<MemorySegment> prevMemSegment = null;
            foreach (var node in TreeNode<MemorySegment>.TraverseToParent(_startingNode, choosen))
            {
                TreeNode<MemorySegment> memSeg = new TreeNode<MemorySegment>(node.Data);
                if(prevMemSegment != null)
                    memSeg.AddChild(prevMemSegment);

                prevMemSegment = memSeg;
            }
            return prevMemSegment;
        }

        private TreeNode<MemorySegment> CreateMemorySegment(
            TreeNode<MemorySegment> parentNode,
            Vector3 buildDirection,
            Vector3 offsetFromParent,
            GeneratorConfigBase.PlacementConfig placementConfig)
        {
            var memSeg = new MemorySegment();
            var curNode = new TreeNode<MemorySegment>(memSeg);
            parentNode.AddChild(curNode);

            // get random meta from available meta providers of current placementConfig
            IEnumerable<MetaBase> allMetas = placementConfig.MetaProviders[0].GetMetas(); // available for current placement config
            for(int i=1; i< placementConfig.MetaProviders.Length;++i)
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
            if (parentNode.Data.ChunkGeometry != null)
            {
                var parentBounds = parentNode.Data.ChunkGeometry.Bounds;
                bounds = CreateBoundsForChild(parentBounds, buildDirection, meta.AABBs[sizeIndex], offsetFromParent);
            }
            else // parentNode is a proxy empty (establishment) node
            {
                bounds = CreateBounds(meta.AABBs[sizeIndex], offsetFromParent);
            }

            var hasCollision = CheckTreeCollisions(bounds, _blueprintTree);
            hasCollision = hasCollision && CheckProjectCollisions(bounds, parentNode);

            memSeg.ChunkGeometry.TopologyType = meta.TopologyType;
            memSeg.ChunkGeometry.Bounds = bounds;
            memSeg.ChunkGeometry.BuildDirection = buildDirection;
            memSeg.ChunkGeometry.Meta = meta.name;
            memSeg.ChunkGeometry.SizeIndex = sizeIndex;
            memSeg.ChunkGeometry.Seed = _rnd.ValueInt();
            memSeg.HasCollision = hasCollision;

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

        private bool CheckProjectCollisions(Bounds checkBounds, TreeNode<MemorySegment> lastParent)
        {
            if (lastParent == null)
                return false;

            var pointer = lastParent;
            while (pointer != _startingNode) 
            {
                var pointerBounds = pointer.Data.ChunkGeometry.Bounds;
                if (pointerBounds.IntersectsEx(checkBounds))
                    return true;
            }
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