using Assets.Plugins.Alg;
using UnityEditor;
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

            ApplyMaterials(semifinishedEnt);

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

        private static void ApplyMaterials(GameObject chunk)
        {
            var colorAtlas = AssetDatabase.LoadAssetAtPath<Material>("Assets/!Prefabs/ColorSchemes/ColorScheme.mat");
            Assert.IsNotNull(colorAtlas);

            var renders = chunk.GetComponentsInChildren<Renderer>();

            foreach (var render in renders)
            {
                render.material = colorAtlas;
            }
        }

        private static void BuildGroupsController(GameObject chunk)
        {
            var groupController = chunk.AddComponent<RootGroupsController>();
            var baseChunk = chunk.GetComponent<ChunkBase>();
            Assert.IsNotNull(groupController);
            Assert.IsNotNull(baseChunk);

            var baseComponents = chunk.GetComponentsInChildren<BaseComponent>(true);
            foreach (var baseComponent in baseComponents)
            {
                baseComponent.Chunk = baseChunk;
                baseComponent.GroupsController = groupController;
            }

            groupController.Init();
        }
    }
}