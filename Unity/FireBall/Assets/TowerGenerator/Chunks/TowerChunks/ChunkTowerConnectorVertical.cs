namespace TowerGenerator
{
    public class ChunkTowerConnectorVertical : ChunkBase
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
