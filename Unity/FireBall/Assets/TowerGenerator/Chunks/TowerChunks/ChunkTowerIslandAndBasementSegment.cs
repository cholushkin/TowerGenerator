namespace TowerGenerator
{
    public class ChunkTowerIslandAndBasementSegment : ChunkBase
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
