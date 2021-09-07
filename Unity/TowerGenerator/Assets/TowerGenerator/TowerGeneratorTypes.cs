using System;

namespace TowerGenerator
{
    // todo: move it to corresponding classes
    // note: one chunk could belong to multiple TopologyType flags
    [Flags]
    public enum TopologyType
    {
        Undefined = 0,
        ChunkPeak = 1,
        ChunkStd = 2,
        ChunkFoundation = 4, // islands, legs, basements, etc.
        ChunkSideEar = 8,
        ChunkBottomEar = 16,
        ChunkTopEar = 32,
        //ChunkConnectorVertical = 64,
        //ChunkConnectorHorizontal = 128,
    }


    // note: one chunk must belong to only one ChunkConformationType, but you could request multiple chunks by combining flags
    [Flags]
    public enum ChunkConformationType 
    {
        Undefined = 0,
        BasicChunkController = 1,
        WaveFuncCollapseChunkController = 2,
        GrowingChunkController = 4,
        MarchingCubesChunkController = 8,
    }

    //[Flags]
    //public enum DynamicGrowSegmentType
    //{
    //    MiddleSegment = 0,
    //    EndingSegment = 1,
    //    StartingSegment = 2,
    //}

}