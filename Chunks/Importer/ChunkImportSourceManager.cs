using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


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
            Assert.IsTrue(sources.Length > 0);

            var fileNameNoExt = Path.GetFileNameWithoutExtension(assetPath);
            return sources.FirstOrDefault(x => fileNameNoExt.StartsWith(x.FbxNameStartFrom));
        }
    }
}