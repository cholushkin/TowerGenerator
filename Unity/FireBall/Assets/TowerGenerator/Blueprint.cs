using GameLib.DataStructures;
using UnityEngine;

namespace TowerGenerator
{

    public class Blueprint
    {
        // Types of data:
        // - Topology
        // - VisualChunk
        // meta, variants enabled
        // - Content
        // 



        public class Segment
        {
            public class TopologySegment
            {
                public enum ChunkType
                {
                    ChunkRoofPeak,
                    ChunkStd,
                    ChunkIslandAndBasement,
                    ChunkSideEar,
                    ChunkBottomEar,
                    ChunkConnectorVertical,
                    ChunkConnectorHorizontal,
                }

                public bool IsOpenedForGenerator;
                public Vector3 Position;
                public Vector3 BuildDirection; 

                public Vector3 AspectRatio;
                public ChunkType ChunkT;
                public bool HasCollision { get; set; }

                public override string ToString()
                {
                    return $"{ChunkT}";
                }
            }

            public class VisualSegment
            {
                // meta
                // decorations
                // biome
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