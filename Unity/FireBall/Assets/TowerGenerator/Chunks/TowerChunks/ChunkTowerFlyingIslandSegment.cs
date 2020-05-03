
namespace TowerGenerator
{
    public class ChunkTowerFlyingIslandSegment : ChunkBase
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

