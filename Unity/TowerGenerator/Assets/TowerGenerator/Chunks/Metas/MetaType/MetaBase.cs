using System;
using UnityEngine;

namespace TowerGenerator
{
    [Serializable]
    public class MetaBase : ScriptableObject
    {
        public string ChunkName;
        public TowerGeneratorSettings.Source Source;
        public ChunkControllerBase.ChunkController ChunkControllerType;
        public TagSet TagSet; // topology, labels, architecture, biome, etc.
        public uint Generation;
        public float ChunkMargin;
        
        public Vector3 AABB; // maximum AABB for the chunk

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}