namespace TowerGenerator
{
    public class ChunkTowerSideEarSegment : ChunkBase
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
