using System;
using System.Collections.Generic;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class SegmentConstructor : MonoBehaviour
    {
        // todo: optional visualizer


        //private Blueprint Blueprint;


        //public GeneratorConfigBase Config { get; private set; }
        //private TopologyGeneratorsManifoldBase _manifold;


        public SegmentConstructor(Blueprint blueprint/*long seed, TreeNode<Blueprint.Segment> genFromNode, GeneratorConfigBase cfg, TopologyGeneratorsManifoldBase manifold*/)
        {
        //    _rnd = new RandomHelper(seed);
        //    State = new GeneratorState(genFromNode);
        //    Config = cfg;
        //    _manifold = manifold;
        }

        //public long GetCurrentSeed()
        //{
        //    return _rnd.GetCurrentSeed();
        //}

        public void Construct(TreeNode<SegmentArchitect.MemorySegment> project)
        {

        }

        //public IEnumerable<TreeNode<Blueprint.Segment>> Build()
        //{
        //    Assert.IsTrue(_varLeafPointers == null || _varLeafPointers.Count == 0);

        //    foreach (var node in _blueprintTree.TraverseDepthFirstPostOrder())
        //    {
        //        var parent = node.Parent;
        //        var created = _constructor.CreateSegment(
        //            parent.Data.CreatedNode,
        //            node.Data.ChunkGeometry
        //        );
        //        node.Data.CreatedNode = created;
        //        yield return created;
        //    }
        //}


        //public abstract IEnumerable<TopGenStep> GenerateTower();

        //public virtual TopGenStep FinalizeTrunk()
        //{
        //    throw new NotImplementedException();
        //    //var nodes = State.SegmentsOpened.AddRange(State.SegmentsActive);
        //    ////Assert.IsFalse(State.IsStillGeneratingTrunk);
        //    //SegmentBuilder segmentBuilder = new SegmentBuilder(this, _rnd.ValueInt());
        //    //var from = State.GetOpenedTrunkNode();

        //    //segmentBuilder.Project(
        //    //    from,
        //    //    Range.One,
        //    //    Vector3.up,
        //    //    Vector3.zero,
        //    //    Config.GetPlacementConfig(Entity.EntityType.ChunkRoofPeak),
        //    //    null,
        //    //    null
        //    //);

        //    //if (segmentBuilder.GetProjectVariantsNumber() == 0) // if we can't propagate up just replace opened segment with roof (ignoring placementConfig)
        //    //{
        //    //    from.Data.Topology.Geometry.EntityType = Entity.EntityType.ChunkRoofPeak;
        //    //    from.Data.Topology.IsOpenedForGenerator = false;
        //    //    return TopGenStep.DoStep(from, TopGenStep.Cmd.SegChangeState);
        //    //}
        //    //segmentBuilder.ApplyProject(0);
        //    //var segment = segmentBuilder.Build().First();
        //    //Assert.IsNotNull(segment);
        //    //return TopGenStep.DoStep(segment, TopGenStep.Cmd.SegSpawn);
        //}


        //public virtual IEnumerable<TopGenStep> FinalizeWholeTower()
        //{
        //    throw new NotImplementedException();
        //    //var opened = State.GetOpenedForGeneration();

        //    //foreach (var openedNode in opened)
        //    //{
        //    //    SegmentBuilder segmentBuilder = new SegmentBuilder(this, _rnd.ValueInt());
        //    //    segmentBuilder.Project(
        //    //        openedNode,
        //    //        Range.One,
        //    //        Vector3.up,
        //    //        Vector3.zero,
        //    //        Config.GetPlacementConfig(Entity.EntityType.ChunkRoofPeak),
        //    //        null,
        //    //        null
        //    //    );

        //    //    if (segmentBuilder.GetProjectVariantsNumber() == 0) // if we can't propagate up just replace opened segment with roof (ignoring placementConfig)
        //    //    {
        //    //        openedNode.Data.Topology.Geometry.EntityType = Entity.EntityType.ChunkRoofPeak;
        //    //        openedNode.Data.Topology.IsOpenedForGenerator = false;
        //    //        yield return TopGenStep.DoStep(openedNode, TopGenStep.Cmd.SegChangeState);
        //    //        continue;
        //    //    }
        //    //    segmentBuilder.ApplyProject(0);
        //    //    var segment = segmentBuilder.Build().First();
        //    //    Assert.IsNotNull(segment);
        //    //    yield return TopGenStep.DoStep(segment, TopGenStep.Cmd.SegSpawn);
        //    //}
        //}

        public TreeNode<Blueprint.Segment> CreateSegment(
            TreeNode<Blueprint.Segment> parent,
            TopologyType topologyType,
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
            GeneratorConfigBase.PlacementConfig placementConfig)
        {
            throw new NotImplementedException();
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

            //State.Created.Push(node);
            return node;
        }


        public static Bounds CreateBoundsForChild(Bounds parentBounds, Vector3 side, Vector3 childSize, Vector3 offset)
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

        
    }



    
}
