using System;
using UnityEngine;

namespace TowerGenerator
{
    [Serializable]
    public class MetaBase : ScriptableObject
    {
        public string ChunkName;
        public ChunkControllerBase.ChunkController ChunkControllerType;
        public TagSet TagSet; // topology, labels, architecture, biome, etc.
        public string[] InConnectorsExpressions; // expressions for in-connectors
        public string[] OutConnectorsExpressions; // expressions for out-connectors
        public uint Generation;
        public Vector3 AABB; // maximum AABB for the chunk

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}