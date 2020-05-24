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
        public class MemorySegment
        {
            public MemorySegment()
            {
            }

            public MemorySegment(TreeNode<Blueprint.Segment> blueprintSegment)
            {
                ChunkGeometry = blueprintSegment.Data.Topology.Geometry;
                CreatedNode = blueprintSegment;
            }

            public Blueprint.Segment.TopologySegment.ChunkGeometry ChunkGeometry = new Blueprint.Segment.TopologySegment.ChunkGeometry();
            public bool HasCollision { get; set; }
            public TreeNode<Blueprint.Segment> CreatedNode;
        }

        //private MemorySegment _steps;
        //private Range _segCount;
        //private MemorySegment _fromStep;
        public int CreatedCount { get; set; }
        private SegmentConstructor _constructor; // for created
        private RandomHelper _rnd;
        //private TreeNode<Blueprint.Segment> _rootNode;
        private TreeNode<MemorySegment> _blueprintTree;
        private List<TreeNode<MemorySegment>> _varLeafPointers;
        private Range _segmentsCount;

        public SegmentArchitect(SegmentConstructor constructor, int seed)
        {
            _constructor = constructor;
            _rnd = new RandomHelper(seed);
        }

        public void Project(
            TreeNode<Blueprint.Segment> from,
            Range segCount,
            Vector3 direction,
            Vector3 offsetFromTrunk,
            GeneratorConfigBase.PlacementConfig firstPlacementConfig,
            GeneratorConfigBase.PlacementConfig intermediatePlacementConfig,
            GeneratorConfigBase.PlacementConfig lastPlacementConfig)
        {
            TreeNode<MemorySegment> proxyMemSegment = null; // todo: implicit constructor
            if (from != null)
                proxyMemSegment = new TreeNode<MemorySegment>(new MemorySegment(from));

            Project(
                proxyMemSegment,
                segCount,
                direction,
                offsetFromTrunk,
                firstPlacementConfig,
                intermediatePlacementConfig,
                lastPlacementConfig
            );
        }

        public void Project(
            TreeNode<MemorySegment> from,
            Range segCount,
            Vector3 direction,
            Vector3 offsetFromTrunk,
            GeneratorConfigBase.PlacementConfig firstPlacementConfig,
            GeneratorConfigBase.PlacementConfig intermediatePlacementConfig,
            GeneratorConfigBase.PlacementConfig lastPlacementConfig)
        {
            Assert.IsTrue(_varLeafPointers == null || _varLeafPointers.Count == 0);
            _segmentsCount = segCount;
            int targetSegmentCount = (int)segCount.From;
            int segmentCounter = 0;
            int branchesCounter = 0;
            _varLeafPointers = new List<TreeNode<MemorySegment>>((int)(segCount.To - segCount.From));
            TreeNode<MemorySegment> nodePointer = from;
            while (segmentCounter != targetSegmentCount)
            {
                // get actual placement config
                var placementConfig = (segmentCounter == 0) ? firstPlacementConfig : intermediatePlacementConfig;
                if (segmentCounter == targetSegmentCount - 1)
                    placementConfig = lastPlacementConfig;

                // create memory segment
                var memSeg = CreateMemorySegment(nodePointer, direction,
                    segmentCounter == 0 ? offsetFromTrunk : Vector3.zero,
                    placementConfig);

                if (memSeg.Data.HasCollision)
                {
                    break;
                }

                ++segmentCounter;
                if (segmentCounter == targetSegmentCount)
                {
                    _varLeafPointers.Add(memSeg);
                    ++branchesCounter;
                    ++targetSegmentCount;
                    continue;
                }
                nodePointer = memSeg;
            }
        }


        public int GetProjectVariantsNumber()
        {
            if (_varLeafPointers == null)
                return 0;
            return _varLeafPointers.Count;
        }

        public bool IsProjectInRange()
        {
            return GetProjectVariantsNumber() >= _segmentsCount.From;
        }

        public void ApplyProject(int index)
        {
            var chosen = _varLeafPointers[index];
            foreach (var varLeafPointer in _varLeafPointers)
            {
                if (varLeafPointer == chosen)
                    continue;
                varLeafPointer.Disconnect();
            }
            _varLeafPointers.Clear();
        }

        public void ApplyProjectRnd()
        {
            var rndIndex = _rnd.Range(0, _varLeafPointers.Count);
            ApplyProject(rndIndex);
        }


        private TreeNode<MemorySegment> CreateMemorySegment(
            TreeNode<MemorySegment> parentMemSegment,
            Vector3 buildDirection,
            Vector3 offsetFromParent,
            GeneratorConfigBase.PlacementConfig placementConfig)
        {
            var memSeg = new MemorySegment();
            var curNode = new TreeNode<MemorySegment>(memSeg);
            parentMemSegment.AddChild(curNode);

            // get segment size
            //if (placementConfig.ChunkSizeStrategy == GeneratorConfigBase.PlacementConfig.SizeStrategy.ChunkRndSize)
            // get random meta of placementConfig.ChunkEntityType
            var filter = new MetaProvider.Filter(
                topology: placementConfig.TopologyType,
                breadthRange: placementConfig.SegmentsSizeBreadth,
                heightRange: placementConfig.SegmentsSizeHeight);
            var meta = _rnd.FromEnumerable(MetaProvider.Instance.GetMetas(filter));

            // get random size
            int sizeIndex = _rnd.Range(0, meta.AABBs.Count);
            _rnd.FromList(meta.AABBs);

            var parentBounds = parentMemSegment.Data.ChunkGeometry.Bounds;
            var childBounds = _constructor.CreateBoundsForChild(parentBounds, buildDirection, meta.AABBs[sizeIndex], offsetFromParent);

            var memoryBounds = _blueprintTree.TraverseDepthFirstPostOrder().Select(x => x.Data.ChunkGeometry.Bounds); // in addition to tree collision check we also need to check for self collision
            var hasCollision = _constructor.CheckCollisions(childBounds, memoryBounds);

            memSeg.ChunkGeometry.TopologyType = placementConfig.TopologyType;
            memSeg.ChunkGeometry.Bounds = childBounds;
            memSeg.ChunkGeometry.BuildDirection = buildDirection;
            memSeg.ChunkGeometry.Meta = meta.name;
            memSeg.ChunkGeometry.SizeIndex = sizeIndex;
            memSeg.ChunkGeometry.Seed = _rnd.ValueInt();

            memSeg.HasCollision = hasCollision;

            return curNode;
        }

        public IEnumerable<TreeNode<Blueprint.Segment>> Build()
        {
            Assert.IsTrue(_varLeafPointers == null || _varLeafPointers.Count == 0);

            foreach (var node in _blueprintTree.TraverseDepthFirstPostOrder())
            {
                var parent = node.Parent;
                var created = _constructor.CreateSegment(
                    parent.Data.CreatedNode,
                    node.Data.ChunkGeometry
                );
                node.Data.CreatedNode = created;
                yield return created;
            }
        }
    }
}