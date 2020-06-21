
namespace TowerGenerator
{
    public class ChunkTowerPeekSegment : ChunkTowerBase
    {
        public override TopologyType GetTopologyType()
        {
            return TopologyType.ChunkPeak;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
