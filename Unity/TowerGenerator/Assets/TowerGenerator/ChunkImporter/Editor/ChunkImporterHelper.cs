
namespace TowerGenerator.ChunkImporter
{
    public static class ChunkImporterHelper
    {
	    public static bool IsChunkPackFbx(string path)
	    {
			return TowerGeneratorConstants.ChunkPackRegex.Match(path).Success;
	    }
    }
}

