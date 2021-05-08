using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerGenerator
{
    [Serializable]
    public class MetaBase : ScriptableObject
    {
        public string ChunkName;
        public TopologyType TopologyType;
        public ChunkConformationType ChunkConformation;
        public ChunkShapeConfigurationType ShapeConfiguration;
        public string[] ChunkClassName; // creature, tower, totem, etc.
        public uint Generation;
        public TagSet TagSet;
        public List<Vector3> AABBs; // each dimension AABB (for combinatorial conformation it's only one value here)

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}