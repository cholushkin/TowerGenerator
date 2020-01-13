using System;
using System.Collections.Generic;
using System.Linq;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public abstract class GeneratorBase
    {
        public class TopGenStep // topology generation elementary step
        {
            public enum Cmd
            {
                SegSpawn,
                SegDestroy,
                SegChangeState,
            }

            public TreeNode<Blueprint.Segment> Segment;
            public Cmd GeneratorCmd;

            public static TopGenStep DoStep(TreeNode<Blueprint.Segment> segment, Cmd cmd)
            {
                Assert.IsNotNull(segment);
                return new TopGenStep
                {
                    Segment = segment,
                    GeneratorCmd = cmd,
                };
            }
        }

        public class GeneratorState
        {
            public Stack<TreeNode<Blueprint.Segment>> Created = new Stack<TreeNode<Blueprint.Segment>>(16);
            public TreeNode<Blueprint.Segment> TrunkDeadlock;

            public List<TreeNode<Blueprint.Segment>> GetOpenedForGeneration()
            {
                return Created.Where(x => x.Data.Topology.IsOpenedForGenerator).ToList();
            }

            public TreeNode<Blueprint.Segment> GetOpenedTrunkNode()
            {
                return Created.FirstOrDefault(x => x.Data.Topology.IsOpenedForGenerator && x.BranchLevel == 0);
            }

            public bool IsStillGeneratingTrunk { get; internal set; }
        }

        public int Iteration { get; internal set; }
        public GeneratorState State { get; protected set; }
        public ConfigBase Config;
        protected RandomHelper _rnd;
        private static readonly Bounds ZeroBounds = new Bounds(Vector3.zero, Vector3.zero);
        private TopologyGeneratorsManifoldBase _manifold;
        protected TreeNode<Blueprint.Segment> _startingNode;

        protected GeneratorBase(long seed, TreeNode<Blueprint.Segment> startingNode, ConfigBase cfg, TopologyGeneratorsManifoldBase manifold)
        {
            _rnd = new RandomHelper(seed);
            State = new GeneratorState();
            Config = cfg;
            _manifold = manifold;
            _startingNode = startingNode;
            State.IsStillGeneratingTrunk = true;
        }

        public long GetCurrentSeed()
        {
            return _rnd.GetCurrentSeed();
        }

        public virtual TopGenStep EstablishTower()
        {
            SegmentBuilder segmentBuilder = new SegmentBuilder(this, _rnd.ValueInt());

            segmentBuilder.Project(
                (TreeNode<SegmentBuilder.MemorySegment>)null, 
                Range.One, 
                Vector3.up, 
                Vector3.zero, 
                Config.GetPlacementConfig(Entity.EntityType.ChunkIslandAndBasement), 
                null, 
                null
            );

            Assert.IsTrue(segmentBuilder.GetProjectVariantsNumber() == 1);
            segmentBuilder.ApplyProject(0);

            var segment = segmentBuilder.Build().First();
            Assert.IsNotNull(segment);
            return TopGenStep.DoStep(segment, TopGenStep.Cmd.SegSpawn);
        }


        public abstract IEnumerable<TopGenStep> GenerateTower();

        public virtual TopGenStep FinalizeTrunk()
        {
            Assert.IsFalse(State.IsStillGeneratingTrunk);
            SegmentBuilder segmentBuilder = new SegmentBuilder(this, _rnd.ValueInt());
            var from = State.GetOpenedTrunkNode();

            segmentBuilder.Project(
                from,
                Range.One,
                Vector3.up,
                Vector3.zero,
                Config.GetPlacementConfig(Entity.EntityType.ChunkRoofPeak),
                null,
                null
            );

            if (segmentBuilder.GetProjectVariantsNumber() == 0) // if we can't propagate up just replace opened segment with roof (ignoring placementConfig)
            {
                from.Data.Topology.Geometry.EntityType = Entity.EntityType.ChunkRoofPeak;
                from.Data.Topology.IsOpenedForGenerator = false;
                return TopGenStep.DoStep(from, TopGenStep.Cmd.SegChangeState);
            }
            segmentBuilder.ApplyProject(0);
            var segment = segmentBuilder.Build().First();
            Assert.IsNotNull(segment);
            return TopGenStep.DoStep(segment, TopGenStep.Cmd.SegSpawn);
        }


        public virtual IEnumerable<TopGenStep> FinalizeWholeTower()
        {
            var opened = State.GetOpenedForGeneration();

            foreach (var openedNode in opened)
            {
                SegmentBuilder segmentBuilder = new SegmentBuilder(this, _rnd.ValueInt());
                segmentBuilder.Project(
                    openedNode,
                    Range.One,
                    Vector3.up,
                    Vector3.zero,
                    Config.GetPlacementConfig(Entity.EntityType.ChunkRoofPeak),
                    null,
                    null
                );

                if (segmentBuilder.GetProjectVariantsNumber() == 0) // if we can't propagate up just replace opened segment with roof (ignoring placementConfig)
                {
                    openedNode.Data.Topology.Geometry.EntityType = Entity.EntityType.ChunkRoofPeak;
                    openedNode.Data.Topology.IsOpenedForGenerator = false;
                    yield return TopGenStep.DoStep(openedNode, TopGenStep.Cmd.SegChangeState);
                    continue;
                }
                segmentBuilder.ApplyProject(0);
                var segment = segmentBuilder.Build().First();
                Assert.IsNotNull(segment);
                yield return TopGenStep.DoStep(segment, TopGenStep.Cmd.SegSpawn);
            }
        }

        public TreeNode<Blueprint.Segment> CreateSegment(
            TreeNode<Blueprint.Segment> parent,
            Entity.EntityType chunkType,
            Vector3 attachDirection,
            Vector3 aspectRatio,
            Vector3 offset)
        {
            throw new NotImplementedException();
        }

        private TreeNode<Blueprint.Segment> CreateMemorySegment(
            TreeNode<Blueprint.Segment> parentSegment,
            Vector3 buildDirection,
            Vector3 offsetFromParent,
            ConfigBase.PlacementConfig placementConfig)
        {
            
        }

        public TreeNode<Blueprint.Segment> CreateSegment( 
            TreeNode<Blueprint.Segment> parent, 
            Blueprint.Segment.TopologySegment.ChunkGeometry chunkGeometry )
        {
            // create segment & node
            var segment = new Blueprint.Segment();
            segment.Topology = new Blueprint.Segment.TopologySegment();
            var node = new TreeNode<Blueprint.Segment>(segment);

            // add created node to the parent
            parent?.AddChild(node);

            // set pos
            //var parentAspRatX = parent?.Data.Topology.Geometry.Bounds.size.x ?? 0;
            //var parentAspRatY = parent?.Data.Topology.Geometry.Bounds.size.y ?? 0;
            //var parentAspRatZ = parent?.Data.Topology.Geometry.Bounds.size.z ?? 0;

            //var xOffset = (parentAspRatX + aspectRatio.x) * attachDirection.x * .5f;
            //var yOffset = (parentAspRatY + aspectRatio.y) * attachDirection.y * .5f;
            //var zOffset = (parentAspRatZ + aspectRatio.z) * attachDirection.z * .5f;

            //segment.Topology.Geometry.Bounds.center = parent == null
            //    ? Vector3.zero
            //    : parent.Data.Topology.Geometry.Bounds.center + new Vector3(xOffset, yOffset, zOffset);
            //segment.Topology.Geometry.Bounds.center += offset;
            //segment.Topology.Geometry.Bounds.size = aspectRatio;
            //segment.Topology.Geometry.EntityType = chunkType;
            //segment.Topology.HasCollision = false;
            //segment.Topology.Geometry.BuildDirection = attachDirection;

            node.Data.Topology.Geometry = chunkGeometry;

            // connections
            Assert.IsTrue(segment.Topology.Connection == Vector3.zero);
            segment.Topology.Connection = -chunkGeometry.BuildDirection;

            State.Created.Push(node);
            return node;
        }


        public Bounds CreateBoundsForChild(Bounds parentBounds, Vector3 side, Vector3 childSize, Vector3 offset)
        {
            var newBounds = new Bounds(parentBounds.center, childSize);
            newBounds.center += new Vector3(
                (parentBounds.size.x + newBounds.size.x) * 0.5f * side.x,
                (parentBounds.size.y + newBounds.size.y) * 0.5f * side.y,
                (parentBounds.size.z + newBounds.size.z) * 0.5f * side.z
            );
            newBounds.center += offset;
            return newBounds;
        }

        public bool CheckCollisions(Bounds checkBounds, IEnumerable<Bounds> collisionBoundsEx = null )
        {
            foreach (var node in _manifold.Pointers.PointerGarbageCollector.TraverseDepthFirstPreOrder())
            {
                var nodeBounds = node.Data.Topology.Geometry.Bounds;
                if (nodeBounds.IntersectsEx(checkBounds))
                    return true;
            }

            if (collisionBoundsEx != null)
                foreach (var node in collisionBoundsEx)
                    if (node.IntersectsEx(checkBounds))
                        return true;
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
