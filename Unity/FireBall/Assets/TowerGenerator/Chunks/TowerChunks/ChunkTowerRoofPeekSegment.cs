
namespace TowerGenerator
{
    public class ChunkTowerRoofPeekSegment : ChunkBase
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
