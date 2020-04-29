using UnityEngine;

namespace TowerGenerator
{
    public abstract class ChunkBase : MonoBehaviour
    {
        public abstract TopologyType GetTopologyType();
    }
}
