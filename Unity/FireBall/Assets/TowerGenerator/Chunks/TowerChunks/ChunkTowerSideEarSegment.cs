namespace TowerGenerator
{
    public class ChunkTowerSideEarSegment : ChunkTowerBase
    {
        public override TopologyType GetTopologyType()
        {
            return TopologyType.ChunkSideEar;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
