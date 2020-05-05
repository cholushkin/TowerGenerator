using System.Collections.Generic;
using Assets.Plugins.Alg;
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
                var metaAsset = ScriptableObject.CreateInstance<MetaChunk>();
                string assetPathAndName = dir + "/" + name + ".m.asset";

                metaAsset.ChunkName = name;
                metaAsset.TopologyType = chunk.GetTopologyType();
                var groupController = chunk.GetComponent<RootGroupsController>();
                Assert.IsNotNull(groupController, "chunk must have a group controller");

                // todo: generation from fbx
                // todo: TagSet

                // AABBs
                for (int i = 0; i < groupController.DimensionStack.GetItemsCount(); ++i)
                    AddAabb(metaAsset, ComputeAABB(groupController, i));

                AssetDatabase.CreateAsset(metaAsset, assetPathAndName);
                AssetDatabase.SaveAssets();
                return metaAsset;
            }
            return null;
        }

        private static Vector3 ComputeAABB(RootGroupsController controller, int i )
        {
            // enable all parts (maybe there are nodes beside DimensionStack
            controller.transform.ForEachChildrenRecursive(t => t.gameObject.SetActive(true));

            // enable only this AABB item
            controller.DimensionStack.DoChoice(i);

            // disable all DimensionsIgnorant nodes on whole chunk
            var dimIgnorant = controller.transform.GetComponentsInChildren<DimensionsIgnorant>(true);
            foreach (var dimensionsIgnorant in dimIgnorant)
                dimensionsIgnorant.gameObject.SetActive(false);

            return controller.CalculateBB().size;
        }

        private static void AddAabb(MetaChunk meta, Vector3 aabb)
        {
            if (meta.AABBs == null)
                meta.AABBs = new List<Vector3>(3);
            meta.AABBs.Add(aabb);
        }
    }
}