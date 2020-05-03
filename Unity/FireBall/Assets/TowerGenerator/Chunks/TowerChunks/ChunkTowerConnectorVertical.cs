namespace TowerGenerator
{
    public class ChunkTowerConnectorVertical : ChunkTowerBase
    {
        public override TopologyType GetTopologyType()
        {
            return TopologyType.ChunkConnectorVertical;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
