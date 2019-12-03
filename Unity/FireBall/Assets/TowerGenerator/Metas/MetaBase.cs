using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerGenerator
{
    [Serializable]
    public class MetaBase : ScriptableObject
    {
        public Entity.EntityType EntityType;
        public string EntName;
        public uint Generation;
        public TagSet TagSet;
        public List<Vector3> AABBs;

        public void AddAABB(Vector3 aabb)
        {
            if(AABBs==null)
                AABBs = new List<Vector3>(3);
            AABBs.Add(aabb);
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}