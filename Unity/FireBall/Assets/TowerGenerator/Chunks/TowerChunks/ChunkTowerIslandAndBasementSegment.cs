namespace TowerGenerator
{
    public class ChunkTowerIslandAndBasementSegment : ChunkTowerBase
    {
        public override TopologyType GetTopologyType()
        {
            return TopologyType.ChunkIsland | TopologyType.ChunkStd;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
