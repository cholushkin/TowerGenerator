using Assets.Plugins.Alg;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkCooker
    {
        public static GameObject Cook(GameObject semifinishedEnt)
        {
            Debug.Log($"Cooking entity: {semifinishedEnt}");

            ExecuteFbxCommands(semifinishedEnt);
            // todo: optme: if (isAnyGroupAdded)
            BuildGroupsController(semifinishedEnt); // tree

            return semifinishedEnt;
        }

        private static void ExecuteFbxCommands(GameObject semifinishedEnt)
        {
            void ProcessCommand(Transform tr)
            {
                var fbxProp = tr.GetComponent<FbxProps>();
                if (fbxProp == null)
                    return;
                FbxCommand.Execute(fbxProp, tr.gameObject);
                tr.gameObject.RemoveComponent<FbxProps>();
            }
            semifinishedEnt.transform.ForEachChildrenRecursive(ProcessCommand);
        }

        private static void BuildGroupsController(GameObject ent)
        {
            var groupController = ent.AddComponent<GroupsController>();
            var baseChunk = ent.GetComponent<ChunkBase>();
            Assert.IsNotNull(groupController);
            Assert.IsNotNull(baseChunk);

            var baseComponents = ent.GetComponentsInChildren<BaseComponent>(true);
            foreach (var baseComponent in baseComponents)
            {
                baseComponent.Chunk = baseChunk;
                baseComponent.GroupsController = groupController;
            }

            groupController.Init();
        }
    }
}