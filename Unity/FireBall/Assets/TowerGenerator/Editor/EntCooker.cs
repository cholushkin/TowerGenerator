using Assets.Plugins.Alg;
using UnityEditor;
using UnityEngine;

namespace TowerGenerator
{
    public static class EntCooker
    {
        private static Transform _entity;
        public static GameObject Cook(GameObject semifinishedEnt)
        {
            Debug.Log($"Cooking entity: {semifinishedEnt}");
            _entity = semifinishedEnt.transform;
            AddScripts(semifinishedEnt);
            // todo: optme: if (isAnyGroupAdded)
            BuildGroupsController(semifinishedEnt); // tree

            return semifinishedEnt;
        }



        private static void AddScripts(GameObject semifinishedEnt)
        {
            void AddScript(Transform tr)
            {
                var fbxProp = tr.GetComponent<FbxProps>();
                if(fbxProp == null)
                    return;
                foreach (var scriptToAdd in fbxProp.ScriptsToAdd)
                    AddComponent(tr.gameObject, scriptToAdd);
            }
            semifinishedEnt.transform.ForEachChildrenRecursive(AddScript);
        }

        private static void AddComponent(GameObject gObj, FbxProps.ScriptToAdd scriptWithParams)
        {
            switch (scriptWithParams.ScriptName)
            {
                // ----- add chunk scripts
                case "ChunkRoofPeak":
                    gObj.AddComponent<ChunkRoofPeak>();
                    break;
                case "ChunkStd":
                    gObj.AddComponent<ChunkStd>();
                    break;
                case "ChunkIslandAndBasement":
                    gObj.AddComponent<ChunkIslandAndBasement>();
                    break;
                case "ChunkSideEar":
                    gObj.AddComponent<ChunkSideEar>();
                    break;
                case "ChunkBottomEar":
                    gObj.AddComponent<ChunkBottomEar>();
                    break;
                case "ChunkConnectorVertical":
                    gObj.AddComponent<ChunkConnectorVertical>();
                    break;
                case "ChunkConnectorHorizontal":
                    gObj.AddComponent<ChunkConnectorHorizontal>();
                    break;

                // ----- add group scripts
                case "GroupSet":
                    gObj.AddComponent<GroupSet>().Configure(_entity, scriptWithParams.ScriptProperties);
                    break;
                case "GroupStack":
                    gObj.AddComponent<GroupStack>().Configure(_entity, scriptWithParams.ScriptProperties);
                    break;
                case "GroupSwitch":
                    gObj.AddComponent<GroupSwitch>().Configure(_entity, scriptWithParams.ScriptProperties);
                    break;
                case "GroupUser":
                    gObj.AddComponent<GroupUser>().Configure(_entity, scriptWithParams.ScriptProperties);
                    break;

                // ----- logic scripts
                case "Connectors":
                    gObj.AddComponent<Connectors>();
                    break;

                default:
                    Debug.LogError($"Unsupported script type: {scriptWithParams.ScriptName}");
                    return;
            }
        }

        private static void BuildGroupsController(GameObject ent)
        {
            var groupController = ent.AddComponent<GroupsController>();
            groupController.BuildImpactTree();
            groupController.CalculateBBMax();
            groupController.CalculateBBMin();
        }

        public static MetaBase CreateMeta(GameObject ent, string dir, string name)
        {
            var entScript = ent.GetComponent<Entity>();
            if (entScript is ChunkStd)
            {
                var asset = ScriptableObject.CreateInstance<MetaChunkStd>();
                string assetPathAndName = dir + "/" + name + ".m.asset";

                asset.EntName = name;

                AssetDatabase.CreateAsset(asset, assetPathAndName);
                AssetDatabase.SaveAssets();
                return asset;
            }

            if (entScript is ChunkRoofPeak)
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