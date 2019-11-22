using UnityEngine;

namespace TowerGenerator
{
    public class Entity : MonoBehaviour
    {
        public enum EntityType
        {
            // building chunks
            ChunkRoofPeak,
            ChunkStd,
            ChunkIslandAndBasement,
            ChunkSideEar,
            ChunkBottomEar,
            ChunkConnectorVertical,
            ChunkConnectorHorizontal,

            // decoration
        }
    }
}