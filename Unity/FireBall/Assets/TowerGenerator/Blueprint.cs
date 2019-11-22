using System.Collections.Generic;
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
                public bool IsOpenedForGenerator;
                public Vector3 Position;
                public Vector3 BuildDirection; 

                public Vector3 AspectRatio;
                public Entity.EntityType EntityType;
                public List<Vector3> Connections = new List<Vector3>(6);
                public bool HasCollision { get; set; }



                public override string ToString()
                {
                    return $"{EntityType}";
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