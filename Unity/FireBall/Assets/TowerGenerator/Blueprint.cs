using GameLib.DataStructures;
using UnityEngine;

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
                    public TopologyType TopologyType;
                    public Bounds Bounds; // position and aspects
                    public Vector3 BuildDirection;
                    public string Meta;
                    public int SizeIndex;
                    public long Seed;
                }

                public ChunkGeometry Geometry;

                public bool IsOpenedForGenerator;
                public Vector3 Connection = Vector3.zero;
                public bool HasCollision { get; set; }

                public override string ToString()
                {
                    return $"{Geometry.TopologyType}";
                }
            }

            public class VisualSegment
            {
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
    }
}