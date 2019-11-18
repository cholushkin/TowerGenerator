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
                public enum SegmentType
                {
                    IslandAndBasement,
                    //Base,
                    Std,
                    RoofPeak,
                    SideEar,
                    BottomEar
                }

                public bool IsOpenedForGenerator;
                public Vector3 Position;
                public Vector3 BuildDirection; 

                public Vector3 AspectRatio;
                public SegmentType SegType;
                public bool HasCollision { get; set; }

                public override string ToString()
                {
                    return $"{SegType}";
                }
            }

            public class VisualSegment
            {
                // meta/ variants picked
                // decoration
            }

            public class ContentSegment
            {
                // monsters, chests, coins/diamonds, path
            }

            //public SegmentData(Vector3 aspectRatio)
            //{
            //    AspectRatio = aspectRatio;
            //}
            //public Vector3 Position;
            //public Vector3 ParentOffsetPosition;
            //public Vector3 AspectRatio;
            //public Transform Representation; // tmp: for visualization
            //public bool hasCollision; // tmp: for visualization

            //// visualizing data

            //// 

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