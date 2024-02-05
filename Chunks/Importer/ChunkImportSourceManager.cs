using System.IO;
using System.Linq;
using UnityEngine;

namespace TowerGenerator
{
    public static class ChunkImportSourceManager
    {
        public static ChunkImportSource GetChunkImportSource(string assetPath)
        {
            var ext = Path.GetExtension(assetPath);
            if (ext != ".blend" && ext != ".fbx")
                return null;

            var sources = Resources.LoadAll<ChunkImportSource>("");
            if (sources.Length == 0)
                return null; 

            return sources.FirstOrDefault(importSource => importSource.IsMyAsset(assetPath));
        }
    }
}