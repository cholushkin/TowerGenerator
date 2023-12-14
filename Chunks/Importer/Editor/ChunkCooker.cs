using System;
using System.Collections.Generic;
using TowerGenerator.FbxCommands;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkCooker
    {
        [Serializable]
        public class ChunkImportState
        {
            public ChunkImportState(string chunkName, ChunkImportSource importSource)
            {
                ChunkName = chunkName;
                ImportSource = importSource;
            }
            public string ChunkName;
            public int CommandsProcessedAmount;

            public int GroupStackAmount;
            public int GroupSetAmount;
            public int GroupSwitchAmount;
            public int CollisionDependentAmount;
            public int DimensionsIgnorantAmount;
            public int IgnoreGroupItemAmount;
            public int SuppressionAmount;
            public int SuppressedByAmount;
            public int InductionAmount;
            public int InducedByAmount;
            public int HiddenAmount;
            public int ColliderAmount;
            public int ConnectorAmount;
            public int IgnoreAddColliderAmount;
            public uint Generation;
            public int CollidersApplied;
            public TagSet ChunkTagSet;
            public ChunkControllerBase.ChunkController ChunkControllerType;
            public readonly ChunkImportSource ImportSource;

            public override string ToString()
            {
                return JsonUtility.ToJson(this, true);
            }
        }

        public static GameObject Cook(ChunkImportSource importSource, GameObject semifinishedEnt, ChunkImportState chunkImportInformation)
        {
            Debug.Log($"Cooking entity: {semifinishedEnt}");

            ExecuteFbxCommands(semifinishedEnt, chunkImportInformation);

            if (importSource.AddColliders)
                ApplyColliders(semifinishedEnt, chunkImportInformation);

            if (importSource.ApplyMaterials)
                ApplyMaterials(semifinishedEnt, chunkImportInformation);

            ConfigureChunkController(semifinishedEnt, chunkImportInformation); // tree

            return semifinishedEnt;
        }

        private static void ExecuteFbxCommands(GameObject semifinishedEnt, ChunkImportState chunkImportInformation)
        {
            var fbxProps = semifinishedEnt.GetComponentsInChildren<FbxProps>(true);
            Assert.IsTrue(fbxProps.Length > 0);
            
            var allCommands = new List<(GameObject, FbxCommandBase)>(fbxProps.Length);

            // Parse all fbxProps
            foreach (var props in fbxProps)
                FbxCommandExecutor.ParseFbxProps(props, allCommands, chunkImportInformation);

            // Execute all commands
            FbxCommandExecutor.ExecuteCommands(allCommands, chunkImportInformation);
        }

        private static void ApplyColliders(GameObject semifinishedEnt, ChunkImportState chunkImportInformation)
        {
            var renders = semifinishedEnt.GetComponentsInChildren<Renderer>();
            Assert.IsTrue(renders.Length > 0, "");

            foreach (var render in renders)
            {
                if (render.gameObject.GetComponent<IgnoreAddCollider>() == null)
                {
                    if (render.gameObject.GetComponent<MeshCollider>() == null)
                    {
                        render.gameObject.AddComponent<MeshCollider>();
                        chunkImportInformation.CollidersApplied++;
                    }
                }
            }
        }

        // be default TowerGenerator applies color scheme material
        // but you can override material on specific objects by "Material" fbx command
        private static void ApplyMaterials(GameObject chunk, ChunkImportState chunkImportInformation)
        {
            var colorAtlas = Resources.Load<Material>("ColorScheme.mat");
            Assert.IsNotNull(colorAtlas, ChunkImporterHelper.AddStateInformation("Can't find ColorScheme.mat material in a project", chunkImportInformation));

            var renders = chunk.GetComponentsInChildren<Renderer>();
            foreach (var render in renders)
            {
                render.material = colorAtlas;
                render.receiveShadows = false;
            }
        }

        private static void ConfigureChunkController(GameObject chunkSemicooked, ChunkImportState chunkImportInformation)
        {
            var chunkController = chunkSemicooked.GetComponent<ChunkControllerBase>();
            Assert.IsNotNull(chunkController, ChunkImporterHelper.AddStateInformation("There is no ChunkControllerBase on chunk. Probably missing ChunkController prop on chunk root in fbx", chunkImportInformation));

            var baseComponents = chunkSemicooked.GetComponentsInChildren<BaseComponent>(true);
            foreach (var baseComponent in baseComponents)
            {
                baseComponent.ChunkController = chunkController;
            }

            chunkController.Init();
        }
    }
}