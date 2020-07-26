using System;

namespace TowerGenerator
{
    // note: one chunk must belong to only one TopologyType, but you can request multiple chunks by combined flags
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


    // note: one chunk must belong to only one ChunkConformationType, but you can request multiple chunks by combined flags
    [Flags]
    public enum ChunkConformationType 
    {
        DimensionsBased = 1,
        Combinatorial = 2,
        Stretchable = 4
    }
}