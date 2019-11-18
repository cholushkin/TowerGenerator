using UnityEditor;
using UnityEngine;

namespace TowerGenerator
{
    public static class EntCooker
    {
        public static GameObject Cook(GameObject semifinishedEnt)
        {
            Debug.Log("Cooking ent");

            BuildGroupsController(semifinishedEnt); // tree

            return semifinishedEnt;
        }

        private static void BuildGroupsController(GameObject ent)
        {
            var groupController = ent.AddComponent<GroupsController>();
            groupController.BuildImpactTree();
            groupController.CalculateBBMax();
            groupController.CalculateBBMin();
        }

        public static MetaBase CreateMeta(Entity ent, string dir, string name)
        {
            if (ent is ChunkStd)
            {
                var asset = ScriptableObject.CreateInstance<MetaChunkStd>();
                string assetPathAndName = dir + "/" + name + ".m.asset";

                asset.EntName = name;

                AssetDatabase.CreateAsset(asset, assetPathAndName);
                AssetDatabase.SaveAssets();
                return asset;
            }

            if (ent is ChunkRoofPeek)
            {
                var asset = ScriptableObject.CreateInstance<MetaChunkRoofPeek>();
                string assetPathAndName = dir + "/" + name + ".m.asset";

                asset.EntName = name;

                AssetDatabase.CreateAsset(asset, assetPathAndName);
                AssetDatabase.SaveAssets();
                return asset;
            }

            return null;
        }
    }
}