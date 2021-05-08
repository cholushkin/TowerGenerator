using System;

namespace TowerGenerator
{
    // note: one chunk must belong to only one TopologyType, but you could request multiple chunks by combining flags
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
        ChunkConnectorVertical = 64,
        ChunkConnectorHorizontal = 128,
    }


    // note: one chunk must belong to only one ChunkConformationType, but you could request multiple chunks by combining flags
    [Flags]
    public enum ChunkConformationType 
    {
        Undefined = 0,
        DimensionsBased = 1,
        Combinatorial = 2,
        DynamicGrow = 4,
        Stretchable = 8
    }

    [Flags]
    public enum ChunkShapeConfigurationType
    {
        Unspecified = 0,
        Vertical = 1,
        Horizontal = 2,
        Fat = 4
    }

    [Flags]
    public enum DynamicGrowSegmentType
    {
        MiddleSegment = 0,
        EndingSegment = 1,
        StartingSegment = 2,
    }
}