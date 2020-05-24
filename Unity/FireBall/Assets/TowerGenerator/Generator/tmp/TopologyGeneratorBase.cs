//using System;
//using System.Collections.Generic;
//using System.Linq;
//using GameLib;
//using GameLib.DataStructures;
//using GameLib.Random;
//using UnityEngine;
//using UnityEngine.Assertions;

//namespace TowerGenerator
//{
//    public abstract class TopologyGeneratorBase : MonoBehaviour
//    {
//        [Serializable]
//        public class TopologyGeneratorConfigBase // base config with some helpers
//        {
//            [Serializable]
//            public class PlacementConfig
//            {
//                public enum PlacementStrategy
//                {
//                    ChunkRndSize,
//                    ChunkMaxSize,
//                    ChunkMinSize
//                }
//                public PlacementStrategy Strategy;
//                public bool IgnoreChunkSizeRestrictions;
//                public Range SegmentsSizeBreadth;
//                public Range SegmentsSizeHeight;
//            }

//            public class ChunkSpecificPlacement
//            {
//                public Entity.EntityType ApplyTo;
//                public PlacementConfig Placement;
//            }


//            [Serializable]
//            public class AllowedDirectionsChances
//            {
//                [Range(0f, 1f)] public float Left;
//                [Range(0f, 1f)] public float Right;
//                [Range(0f, 1f)] public float Up;
//                [Range(0f, 1f)] public float Down;
//                [Range(0f, 1f)] public float Forward;
//                [Range(0f, 1f)] public float Back;
//            }

//            public Range TrunkSegmentsCount;
//            public PlacementConfig DefaultPlacement;
//            public List<ChunkSpecificPlacement> ChunkSpecific;
//            public AllowedDirectionsChances AllowedDirections = new AllowedDirectionsChances();


//            private float[] _dirChances = new float[6];

//            private static readonly Vector3[] _directions =
//            {
//                Vector3.left, Vector3.right, Vector3.up,
//                Vector3.down, Vector3.forward, Vector3.back,
//            };

//            public Vector3 GetRndPropagationDir(ref RandomHelper rnd)
//            {
//                _dirChances[0] = AllowedDirections.Left;
//                _dirChances[1] = AllowedDirections.Right;
//                _dirChances[2] = AllowedDirections.Up;
//                _dirChances[3] = AllowedDirections.Down;
//                _dirChances[4] = AllowedDirections.Forward;
//                _dirChances[5] = AllowedDirections.Back;

//                var rndDirIndex = rnd.SpawnEvent(_dirChances);
//                return _directions[rndDirIndex];
//            }

//            public Vector3 GetRndSegSize(ref RandomHelper rnd, Entity.EntityType entType)
//            {
//                var placementCfg = ChunkSpecific.FirstOrDefault(e => e.ApplyTo == entType)?.Placement ?? DefaultPlacement;
//                var breadth = rnd.FromRange(placementCfg.SegmentsSizeBreadth);
//                var height = rnd.FromRange(placementCfg.SegmentsSizeHeight);
//                return new Vector3(breadth, height, breadth);
//            }

//            public override string ToString()
//            {
//                return JsonUtility.ToJson(this, true);
//            }
//        }

//        public class TopGenStep // topology generation step
//        {
//            public enum VisualizationCmd
//            {
//                SegSpawn,
//                SegDestroy,
//                SegChangeState,
//            }

//            public TreeNode<Blueprint.Segment> Segment;
//            public VisualizationCmd VisCmd;

//            public static TopGenStep DoStep(TreeNode<Blueprint.Segment> segment, VisualizationCmd cmd)
//            {
//                Assert.IsNotNull(segment);
//                return new TopGenStep
//                {
//                    Segment = segment,
//                    VisCmd = cmd,
//                };
//            }
//        }

//        public class GeneratorState
//        {
//            public Stack<TreeNode<Blueprint.Segment>> Created = new Stack<TreeNode<Blueprint.Segment>>(16);
//            public TreeNode<Blueprint.Segment> Deadlock;

//            public List<TreeNode<Blueprint.Segment>> GetOpenedForGeneration()
//            {
//                return Created.Where(x => x.Data.Topology.IsOpenedForGenerator).ToList();
//            }
//        }

//        public GeneratorState PrevState { get; protected set; }
//        public GeneratorState CurrentState { get; protected set; }

//        public uint CurrentSeed => (uint)_rnd.GetCurrentSeed();

//        protected RandomHelper _rnd = new RandomHelper(-1);
//        private TopologyGeneratorsManifold _manifold;

//        public virtual void PrepareGenerate(TopologyGeneratorsManifold manifold)
//        {
//            _manifold = manifold;
//            Assert.IsNotNull(_manifold);
//        }

//        public abstract IEnumerable<TopGenStep> Generate(uint seed, GeneratorState prevState);

//        public virtual IEnumerable<TopGenStep> Finalize(uint seed, GeneratorState prevState)
//        {
//            PrevState = prevState;
//            ClearCurrentState();
//            _rnd.SetCurrentSeed(seed);
//            var opened = prevState.GetOpenedForGeneration();

//            foreach (var openedNode in opened)
//            {
//                Assert.IsNotNull(openedNode);
//                Assert.IsTrue(openedNode.Data.Topology.IsOpenedForGenerator);
//                openedNode.Data.Topology.EntityType = Entity.EntityType.ChunkRoofPeak;
//                openedNode.Data.Topology.IsOpenedForGenerator = false;
//                yield return TopGenStep.DoStep(openedNode, TopGenStep.VisualizationCmd.SegChangeState);
//            }
//        }

//        public abstract TopologyGeneratorConfigBase GetConfig();

//        protected void ClearCurrentState()
//        {
//            CurrentState = new GeneratorState();
//        }

//        public Blueprint.Segment.TopologySegment.ChunkGeometry GetNextSegmentGeometry(
//            TopologyGeneratorConfigBase.PlacementConfig placementConfig, 
//            Bounds parentBounds, 
//            Vector3 buildDirection, 
//            Vector3 offset, // instead of connecting close to parent (side to side) use this offset of the parent
//            IEnumerable<Bounds> collisionBoundsEx = null)
//        {
//            var geom = new Blueprint.Segment.TopologySegment.ChunkGeometry();

//            // get range of allowed AABB by cfg 

//            // get metas inside that range

//            return geom;

//        }



//        // return random from min to max
//        Vector3 GetRndSegmentSize(Vector3 from, Vector3 to)
//        {
//            var result = new Vector3();
//            result.x = _rnd.Range(from.x, to.x);
//            result.z = result.x;
//            result.y = _rnd.Range(from.y, to.y);
//            return result;
//        }

//        bool IsSegmentSizeBetweenMinMax(Range segMinMax, Vector3 segSize)
//        {
//            if (segSize.x < segMinMax.From)
//                return false;
//            if (segSize.x > segMinMax.To)
//                return false;
//            if (segSize.y < segMinMax.From)
//                return false;
//            if (segSize.y > segMinMax.To)
//                return false;
//            if (segSize.z < segMinMax.From)
//                return false;
//            if (segSize.z > segMinMax.To)
//                return false;
//            return true;
//        }

//        public Bounds CreateBoundsForChild(Bounds parentBounds, Vector3 side, Vector3 childSize, Vector3 offset, float childScale = 1f)
//        {
//            var newBounds = new Bounds(parentBounds.center, childSize * childScale);
//            newBounds.center += new Vector3(
//                (parentBounds.size.x + newBounds.size.x) * 0.5f * side.x,
//                (parentBounds.size.y + newBounds.size.y) * 0.5f * side.y,
//                (parentBounds.size.z + newBounds.size.z) * 0.5f * side.z
//            );
//            newBounds.center += offset;
//            return newBounds;
//        }

//        public Bounds CreateBounds(TreeNode<Blueprint.Segment> node)
//        {
//            Assert.IsNotNull(node);
//            return new Bounds(node.Data.Topology.Geometry.Position, node.Data.Topology.Geometry.AspectRatio);
//        }

//        public Vector3 GetNextSegmentRndFitSize(Range segmentSizeRange,
//            TreeNode<Blueprint.Segment> parent, Vector3 buildDirection, Vector3 offset)
//        {
//            Assert.IsNotNull(parent);
//            return GetNextSegmentRndFitSize(segmentSizeRange, CreateBounds(parent), buildDirection, offset);
//        }

//        public Vector3 GetNextSegmentRndFitSize(
//            Range segmentSizeRange,
//            Bounds parentBounds, 
//            Vector3 buildDirection, Vector3 offset, IEnumerable<Bounds> collisionBoundsEx = null)
//        {
//            // max possible segment size
//            var maxSegSize = Vector3.one * segmentSizeRange.To;
//            var minSegSize = Vector3.one * segmentSizeRange.From;

//            //Debug.Log($"maxseg{maxSegSize} minseg{minSegSize} buildDirection{buildDirection}");

//            // start from max segment size
//            Bounds currentBB = CreateBoundsForChild(parentBounds, buildDirection, maxSegSize, offset, 1f);

//            // get CollidedList
//            List<Bounds> CollidedList = new List<Bounds>(8);
//            foreach (var node in _manifold.Pointers.PointerGarbageCollector.TraverseDepthFirstPreOrder())
//            {
//                var nodeBounds = new Bounds(node.Data.Topology.Geometry.Position, node.Data.Topology.Geometry.AspectRatio);
//                if (nodeBounds.IntersectsEx(currentBB))
//                    CollidedList.Add(nodeBounds);
//            }

//            foreach (var bounds in collisionBoundsEx)
//                if (bounds.IntersectsEx(currentBB))
//                    CollidedList.Add(bounds);

//            if (CollidedList.Count == 0)
//            {
//                //Debug.Log("just normal result (no collisions)");
//                return GetRndSegmentSize(minSegSize, maxSegSize);
//            }

//            // decrease currentBB while has no collisions
//            //Debug.Log("Decreasing");
//            var hasCollision = true;
//            do
//            {
//                var smallerBounds = CreateBoundsForChild(parentBounds, buildDirection, currentBB.size, offset, 0.75f);
//                if (!IsSegmentSizeBetweenMinMax(segmentSizeRange, smallerBounds.size))
//                    break;
//                hasCollision = false;
//                currentBB = smallerBounds;
//                foreach (var bounds in CollidedList)
//                {
//                    if (bounds.IntersectsEx(currentBB))
//                        hasCollision = true;
//                }
//            } while (hasCollision);

//            if (!hasCollision)
//            {
//                //Debug.Log($"result is between = {minSegSize} and {currentBB.size}");
//                return GetRndSegmentSize(minSegSize, currentBB.size);
//            }
//            //Debug.Log($"result = {Vector3.zero}");
//            return Vector3.zero;
//        }

//        //protected void DisconnectSegment(TreeNode<Blueprint.Segment> segment)
//        //{
//        //    Assert.IsNotNull(segment);
//        //    Assert.IsTrue(segment.IsLeaf);
//        //    segment.Disconnect();
//        //}

//        public TreeNode<Blueprint.Segment> CreateSegment(
//            TreeNode<Blueprint.Segment> parent,
//            Vector3 attachDirection, Vector3 aspectRatio, Vector3 offset)
//        {
//            // create segment & node
//            var segment = new Blueprint.Segment();
//            segment.Topology = new Blueprint.Segment.TopologySegment();
//            var node = new TreeNode<Blueprint.Segment>(segment);

//            if (parent != null)
//            {
//                parent.AddChild(node); // add to parent
//            }

//            // set pos
//            var parentAspRatX = parent?.Data.Topology.Geometry.AspectRatio.x ?? 0;
//            var parentAspRatY = parent?.Data.Topology.Geometry.AspectRatio.y ?? 0;
//            var parentAspRatZ = parent?.Data.Topology.Geometry.AspectRatio.z ?? 0;

//            var xOffset = (parentAspRatX + aspectRatio.x) * attachDirection.x * .5f;
//            var yOffset = (parentAspRatY + aspectRatio.y) * attachDirection.y * .5f;
//            var zOffset = (parentAspRatZ + aspectRatio.z) * attachDirection.z * .5f;

//            segment.Topology.Geometry.Position = parent == null
//                ? Vector3.zero
//                : parent.Data.Topology.Geometry.Position + new Vector3(xOffset, yOffset, zOffset);
//            segment.Topology.Geometry.Position += offset;
//            segment.Topology.Geometry.AspectRatio = aspectRatio;
//            segment.Topology.EntityType = Entity.EntityType.ChunkStd;
//            segment.Topology.HasCollision = false;
//            segment.Topology.BuildDirection = attachDirection;

//            // connections
//            Assert.IsTrue(segment.Topology.Connection == Vector3.zero);
//            segment.Topology.Connection = -attachDirection;

//            CurrentState.Created.Push(node);
//            return node;
//        }

//        protected bool CheckCollisions(TreeNode<Blueprint.Segment> checkNode)
//        {
//            Bounds checkNodeBounds = new Bounds(checkNode.Data.Topology.Geometry.Position, checkNode.Data.Topology.Geometry.AspectRatio);

//            foreach (var node in _manifold.Pointers.PointerGarbageCollector.TraverseDepthFirstPreOrder())
//            {
//                if (node == checkNode)
//                    continue;
//                var nodeBounds = new Bounds(node.Data.Topology.Geometry.Position, node.Data.Topology.Geometry.AspectRatio);
//                if (nodeBounds.IntersectsEx(checkNodeBounds))
//                    return true;
//            }
//            return false;
//        }

//        #region High-level helpers
//        // todo: GenerateCross(or flower)
//        // todo: bypass 
//        // todo: sprawling

//        //protected IEnumerable<TopGenStep> GenerateLine(Range segSizeMinMax,
//        //    TreeNode<Blueprint.Segment> parent, int segCount, Vector3 direction)
//        //{
//        //    for (int i = 0; i < segCount; ++i)
//        //    {
//        //        Vector3 fitSize = GetNextSegmentRndFitSize(segSizeMinMax, parent, direction, Vector3.zero);
//        //        if (fitSize.x < segSizeMinMax.From)
//        //            yield break;
//        //        parent = CreateSegment(parent, direction, fitSize, Vector3.zero);
//        //        yield return TopGenStep.DoStep(CurrentState.Created.Peek(), TopGenStep.VisualizationCmd.SegSpawn);
//        //    }
//        //}

//        protected TreeNode<Blueprint.Segment> CreateOriginIsland(TopologyGeneratorConfigBase config)
//        {
//            var islandSize = config.GetRndSegSize(ref _rnd, Entity.EntityType.ChunkIslandAndBasement);
//            var islandSegment = CreateSegment(null, Vector3.zero, islandSize, Vector3.zero);
//            islandSegment.Data.Topology.EntityType = Entity.EntityType.ChunkIslandAndBasement;
//            islandSegment.Data.Topology.IsOpenedForGenerator = true;
//            CurrentState.Created.Push(islandSegment);
//            return islandSegment;
//        }
//        #endregion
//    }

//    public static class BoundExtension
//    {
//        public static bool IntersectsEx(this Bounds b, Bounds bounds)
//        {
//            var hasCollision =
//                (double)b.min.x < (double)bounds.max.x && 
//                (double)b.max.x > (double)bounds.min.x && 
//                (double)b.min.y < (double)bounds.max.y && 
//                (double)b.max.y > (double)bounds.min.y && 
//                (double)b.min.z < (double)bounds.max.z && 
//                (double)b.max.z > (double)bounds.min.z;
//            if (hasCollision)
//            {
//                var delta = (b.center - bounds.center);
//                var ix = Mathf.Abs(delta.x) - (b.size.x + bounds.size.x) * 0.5f;
//                var iy = Mathf.Abs(delta.y) - (b.size.y + bounds.size.y) * 0.5f;
//                var iz = Mathf.Abs(delta.z) - (b.size.z + bounds.size.z) * 0.5f;

//                if (ix >= 0f) // has no intrusion by x component
//                    ix = 0f;
//                if (iy >= 0f)
//                    iy = 0f;
//                if (iz >= 0f)
//                    iz = 0f;

//                double TOLERANCE = 0.0001f;
//                if (Math.Abs(ix) > TOLERANCE && Math.Abs(iy) > TOLERANCE && Math.Abs(iz) > TOLERANCE)
//                    return true;
//            }
//            return false;
//        }
//    }
//}