using System;
using UnityEngine;

namespace TowerGenerator
{
    public class Entity : MonoBehaviour
    {
        [Flags]
        public enum EntityType
        {
            Undefined = 0,
            
            // building chunks
            ChunkRoofPeak = 1,
            ChunkStd = 2,
            ChunkIslandAndBasement = 4,
            ChunkSideEar = 8,
            ChunkBottomEar = 16,
            ChunkConnectorVertical = 32,
            ChunkConnectorHorizontal = 64,

            // decoration
        }
    }
}