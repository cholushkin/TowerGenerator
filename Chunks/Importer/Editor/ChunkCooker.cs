using System;
using Assets.Plugins.Alg;
using TowerGenerator.FbxCommands;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkCooker
    {
        [Serializable]
        public class ChunkImportInformation
        {
            public ChunkImportInformation(string chunkName)
            {
                ChunkName = chunkName;
            }
            public string ChunkName;
            public int CommandsProcessedAmount;

            public int GroupStackAmount;
            public int GroupSetAmount;
            public int GroupSwitchAmount;
            public int CollisionDependentAmount;
            public int DimensionsIgnorantAmount;
            public int SuppressionAmount;
            public int SuppressedByAmount;
            public int InductionAmount;
            public int InducedByAmount;
            public int HiddenAmount;
            public int ConnectorAmount;
            public int IgnoreAddColliderAmount;
            public uint Generation;
            public TagSet ChunkTagSet;
            public ChunkControllerBase.ChunkController ChunkControllerType;

            public override string ToString()
            {
                return JsonUtility.ToJson(this, true);
            }
        }

        public static GameObject Cook(TowerGeneratorImportSource source, GameObject semifinishedEnt, ChunkImportInformation chunkImportInformation)
        {
            Debug.Log($"Cooking entity: {semifinishedEnt}");

            ExecuteFbxCommands(semifinishedEnt, chunkImportInformation);

            if(source.AddColliders)
                ApplyColliders(semifinishedEnt);

            if (source.ApplyMaterials)
                ApplyMaterials(semifinishedEnt);

            ConfigureChunkController(semifinishedEnt); // tree

            return semifinishedEnt;
        }

        private static void ExecuteFbxCommands(GameObject semifinishedEnt, ChunkImportInformation chunkImportInformation)
        {
            void ProcessCommand(Transform tr)
            {
                var fbxProp = tr.GetComponent<FbxProps>();
                if (fbxProp == null)
                    return;
                FbxCommandExecutor.Execute(fbxProp, tr.gameObject, chunkImportInformation);
                tr.gameObject.RemoveComponent<FbxProps>();
            }
            semifinishedEnt.transform.ForEachChildrenRecursive(ProcessCommand);
        }

        private static void ApplyColliders(GameObject semifinishedEnt)
        {
            var renders = semifinishedEnt.GetComponentsInChildren<Renderer>();

            foreach (var render in renders)
            {
                if (render.gameObject.GetComponent<IgnoreAddCollider>() == null)
                {
                    if(render.gameObject.GetComponent<MeshCollider>()==null)
                        render.gameObject.AddComponent<MeshCollider>();
                }
            }
        }

        // be default TowerGenerator applies color scheme material
        // but you can override material on specific objects by "Material" fbx command
        private static void ApplyMaterials(GameObject chunk)
        {
            var colorAtlas = Resources.Load<Material>("ColorScheme.mat");
            Assert.IsNotNull(colorAtlas);

            var renders = chunk.GetComponentsInChildren<Renderer>();

            foreach (var render in renders)
            {
                render.material = colorAtlas;
                render.receiveShadows = false;
            }
        }

        private static void ConfigureChunkController(GameObject chunk)
        {
            var chunkController = chunk.GetComponent<ChunkControllerBase>();
            Assert.IsNotNull(chunkController);

            var baseComponents = chunk.GetComponentsInChildren<BaseComponent>(true);
            foreach (var baseComponent in baseComponents)
            {
                baseComponent.ChunkController = chunkController;
            }

            chunkController.Validate();
        }
    }
}