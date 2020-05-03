
namespace TowerGenerator
{
    public class ChunkTowerStandardSegment : ChunkBase
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
