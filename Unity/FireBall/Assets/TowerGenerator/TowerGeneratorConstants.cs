using System.Text.RegularExpressions;
using UnityEngine;

namespace TowerGenerator
{

    public static class TowerGeneratorConstants
    {
        // player
        // physics
        // blocks
        public static readonly Vector3 ConnectorMargin = Vector3.one;
        public const float ConnectorAspect = 3f;

        #region ---------- Paths ---------- 

        // allowed values for ChunkPack asset path:
        // * Assets/!Import/ChunkFbx/Chunks_0.fbx - minimal valid path
        // * Assets/!Import/ChunkFbx/Chunks_001.fbx - any number at the end
        // * Assets/!Import/ChunkFbx/Chunks_small_001.fbx - name of the chunk pack
        // * Assets/!Import/ChunkFbx/Monsters/Cute/Chunks_small_001.fbx - any amount of subfolders under ChunkFbx
        public static readonly Regex ChunkPackRegex = new Regex(@"(Assets/!Import/ChunksFbx/)([a-zA-Z0-9]+/)*(Chunks_)([a-zA-Z0-9]+_)?(\d+).fbx"); // regex to much chunks fbx 


        public const string PathChunks = "Assets/!Prefabs/Resources/Chunks";
        #endregion
    }
}