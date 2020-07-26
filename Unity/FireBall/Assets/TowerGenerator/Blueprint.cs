using GameLib.DataStructures;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{

    public class Blueprint
    {
        public class Segment
        {
            public class TopologySegment
            {
                public class ChunkGeometry
                {
                    public Bounds Bounds; // position and aspects
                    public Vector3 BuildDirection;
                    public MetaBase Meta;
                    public int SizeIndex;
                }

                public ChunkGeometry Geometry;

                public Vector3 Connection = Vector3.zero;
                public bool HasCollision { get; set; }

                public override string ToString()
                {
                    return $"{Geometry.Meta.name}";
                }
            }

            public class VisualSegment
            {
                public Transform ChunkTransform;
                public long Seed;
                // transform of created chunk
                // decorations
                // biome
                // color scheme
            }

            public class ContentSegment
            {
                // monsters, chests, coins/diamonds, path
            }

            public override string ToString()
            {
                return $"{Topology}";
            }

            public TopologySegment Topology;
            public VisualSegment Visual;
            public ContentSegment Content;
        }

        public TreeNode<Segment> Tree;


        //public static Segment mapper(SegmentArchitect.MemorySegment memSegData)
        //{
        //    var blueprintSegment = new Segment();
        //    blueprintSegment.Topology = new Segment.TopologySegment();
        //    blueprintSegment.Topology.Geometry = memSegData.ChunkGeometry;
        //    return blueprintSegment;
        //}

        public void AddSubtree(
            TreeNode<Segment> parent, // to which segment of a blueprint tree need to add
            Vector3 connectorSideOnParent, // side of parent to connect subtree
            TreeNode<Segment> subtree)
        {
            parent?.AddChild(subtree);
            if (parent == null)
                Tree = subtree;
        }

        //public static TreeNode<Segment> CreateTopologySegments(
        //    TreeNode<Segment> from,
        //    Vector3 connectorSideOnParent, 
        //    TreeNode<SegmentArchitect.MemorySegment> project)
        //{
        //    var blueprintChain = TreeNode<SegmentArchitect.MemorySegment>.Convert(project, mapper);
        //    from?.AddChild(blueprintChain);
        //    return blueprintChain;
        //}

        public static TreeNode<Segment> CreateTopologySegment(
            TreeNode<Blueprint.Segment> parent,
            Vector3 connectorSideOnParent,
            Blueprint.Segment.TopologySegment.ChunkGeometry chunkGeometry)
        {
            Assert.IsNotNull(chunkGeometry);
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
    }
}