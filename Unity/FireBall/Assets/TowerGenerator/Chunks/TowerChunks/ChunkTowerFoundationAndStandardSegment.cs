namespace TowerGenerator
{
    public class ChunkTowerFoundationAndStandardSegment : ChunkTowerBase
    {
        public override TopologyType GetTopologyType()
        {
            return TopologyType.ChunkFoundation| TopologyType.ChunkStd;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
