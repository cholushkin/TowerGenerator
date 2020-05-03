using UnityEngine;

namespace TowerGenerator
{
    public abstract class ChunkBase : BaseComponent
    {
        // todo: tags


        public abstract TopologyType GetTopologyType();
    }
}
