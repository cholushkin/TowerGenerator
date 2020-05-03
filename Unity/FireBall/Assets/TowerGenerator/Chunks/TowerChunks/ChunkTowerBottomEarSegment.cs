namespace TowerGenerator
{
    public class ChunkTowerBottomEarSegment : ChunkTowerBase
    {
        public override TopologyType GetTopologyType()
        {
            return TopologyType.ChunkBottomEar;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
