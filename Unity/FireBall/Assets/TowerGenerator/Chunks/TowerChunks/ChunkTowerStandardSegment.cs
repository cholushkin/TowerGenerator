
namespace TowerGenerator
{
    public class ChunkTowerStandardSegment : ChunkTowerBase
    {
        public override TopologyType GetTopologyType()
        {
            return TopologyType.ChunkStd;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
