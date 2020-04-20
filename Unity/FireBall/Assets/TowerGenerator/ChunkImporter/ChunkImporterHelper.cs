using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkImporterHelper
    {
		// possible values for ChunkPack asset path:
		// * Assets/!Import/ChunkFbx/Chunks_0.fbx - minimal valid path
		// * Assets/!Import/ChunkFbx/Chunks_001.fbx - any number at the end
		// * Assets/!Import/ChunkFbx/Chunks_small_001.fbx - name of the chunk pack
		// * Assets/!Import/ChunkFbx/Monsters/Cute/Chunks_small_001.fbx - any amount of subfolders under ChunkFbx
		public static readonly Regex ChunkPackRegex = new Regex(@"(Assets/!Import/ChunksFbx/)([a-zA-Z0-9]+/)*(Chunks_)([a-zA-Z0-9]+_)?(\d+).fbx"); // regex to much chunks fbx 

		public const string FbxChunksNameFormat = "Chunks.*";
		// Property names
		public const string PropNameMinObjectsSelected = "MinObjectsSelected";
        public const string PropNameMaxObjectsSelected = "MaxObjectsSelected";
        public const string PropNameAddScript = "AddScript";
        public const string PropNamePropagatedTo = "PropagatedTo";
        public const string PropNameHost = "Host";
        public const string PropNameCollisionCheck = "CollisionCheck";
#if UNITY_EDITOR
        public static bool CheckPropNames(List<FbxProps.ScriptToAdd.ScriptProperty> scriptProperties, params string[] propNamesPossible)
        {
            foreach (var sp in scriptProperties)
                if (!propNamesPossible.Contains(sp.PropName))
                {
                    Debug.Log($"Bad property name: {sp.PropName}");
                    return false;
                }

            return true;
        }
#endif

	    public static bool IsChunkPackFbx(string path)
	    {
			return ChunkPackRegex.Match(path).Success;
	    }

        public static bool ParseFloat(string propValue, out float result)
        {
            return Single.TryParse(propValue, out result);
        }

        public static bool ParseInt(string propValue, out int result)
        {
            return Int32.TryParse(propValue, out result);
        }
    }
}

