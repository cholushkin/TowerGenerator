using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkMetaCooker
    {
        public static MetaBase Cook(GameObject chunkObject, string dir, string name)
        {
            var chunk = chunkObject.GetComponent<ChunkBase>();

            if (chunk is ChunkTowerBase)
            {
                var asset = ScriptableObject.CreateInstance<MetaChunk>();
                string assetPathAndName = dir + "/" + name + ".m.asset";

                asset.ChunkName = name;
                asset.TopologyType = chunk.GetTopologyType();
                var groupController = chunk.GetComponent<GroupsController>();
                Assert.IsNotNull(groupController, "chunk must have a group controller");

                // todo: generation from fbx
                // todo: TagSet

                // AABBs
                for (int i = 0; i < groupController.DimensionStack.GetItemsCount(); ++i)
                {
                    groupController.DimensionStack.DoChoice(i);
                    AddAabb(asset, groupController.CalculateBB().size);
                }

                AssetDatabase.CreateAsset(asset, assetPathAndName);
                AssetDatabase.SaveAssets();
                return asset;
            }
            return null;
        }

        private static void AddAabb(MetaChunk meta, Vector3 aabb)
        {
            if (meta.AABBs == null)
                meta.AABBs = new List<Vector3>(3);
            meta.AABBs.Add(aabb);
        }
    }
}