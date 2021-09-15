using System.IO;
using System.Linq;
using UnityEngine;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkImporterHelper
    {
	    public static bool IsNeededToImportChunkPackFbx(string path)
        {
            var needImport = true; // has blend or fbx extension and is in the one of the source folder
            var extension = Path.GetExtension(path);
            needImport &= extension.Equals(".blend") || extension.Equals(".fbx");
            if (!needImport)
                return false;

            var inOneOfTheSource = false;
            var settings = ScriptableObjectUtility.GetInstanceOfSingletonScriptableObject<TowerGeneratorSettings>();
            foreach (var source in settings.Sources)
                if (path.StartsWith(source.ImportPath))
                {
                    inOneOfTheSource = true;
                    break;
                }

            needImport &= inOneOfTheSource;
			return needImport;
	    }

        public static bool IsObjectIgnored(GameObject obj)
        {
            var fbxProbs = obj.GetComponent<FbxProps>();
            return (fbxProbs == null || (fbxProbs.Properties.FirstOrDefault(x => x.Name == "IgnoreImport") != null));
        }

        public static TowerGeneratorSettings.Source GetSource(string assetPath)
        {
            var settings = ScriptableObjectUtility.GetInstanceOfSingletonScriptableObject<TowerGeneratorSettings>();
            var source = settings.Sources.FirstOrDefault(x=> assetPath.StartsWith(x.ImportPath));
            return source;
        }
    }

}

