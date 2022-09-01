﻿using System;
using UnityEngine;

namespace TowerGenerator
{
    [Serializable]
    public class MetaBase : ScriptableObject
    {
        public string ChunkName;
        public ChunkControllerBase.ChunkController ChunkControllerType;
        public TagSet TagSet; // topology, labels, architecture, biome, etc.
        public uint Generation;
        public float ChunkMargin;
        
        public Vector3 AABB; // Axed-aligned bounding box
        public Vector3 OBB; // Oriented bounding box
        public ChunkImportSource ImportSource;

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}