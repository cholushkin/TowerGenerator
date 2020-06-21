using System;
using UnityEngine;

namespace TowerGenerator
{
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
}