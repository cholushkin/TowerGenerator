using UnityEngine;

namespace TowerGenerator
{
    public abstract class ChunkBase : BaseComponent
    {
        public abstract TopologyType GetTopologyType();
    }
}
