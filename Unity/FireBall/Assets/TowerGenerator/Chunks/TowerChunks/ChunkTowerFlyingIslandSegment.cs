
namespace TowerGenerator
{
    public class ChunkTowerFlyingIslandSegment : ChunkTowerBase
    {
        public override TopologyType GetTopologyType()
        {
            return TopologyType.ChunkIsland;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

