
namespace TowerGenerator
{
    public class ChunkTowerRoofPeekSegment : ChunkTowerBase
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
