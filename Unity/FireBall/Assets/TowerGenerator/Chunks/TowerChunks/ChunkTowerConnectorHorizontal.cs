namespace TowerGenerator
{
    public class ChunkTowerConnectorHorizontal : ChunkBase
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
