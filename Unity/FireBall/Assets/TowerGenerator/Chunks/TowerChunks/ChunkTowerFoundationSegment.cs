
namespace TowerGenerator
{
    public class ChunkTowerFoundationSegment : ChunkTowerBase
    {
        public override TopologyType GetTopologyType()
        {
            return TopologyType.ChunkFoundation;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

