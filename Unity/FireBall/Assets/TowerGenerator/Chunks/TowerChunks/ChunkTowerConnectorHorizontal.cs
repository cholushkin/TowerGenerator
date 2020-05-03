namespace TowerGenerator
{
    public class ChunkTowerConnectorHorizontal : ChunkTowerBase
    {
        public override TopologyType GetTopologyType()
        {
            return TopologyType.ChunkConnectorHorizontal;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
