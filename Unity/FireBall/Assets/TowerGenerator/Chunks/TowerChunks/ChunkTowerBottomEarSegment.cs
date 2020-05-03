namespace TowerGenerator
{
    public class ChunkTowerBottomEarSegment : ChunkBase
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
