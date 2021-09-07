using System;
using System.Collections.Generic;
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
                ConformationType = new Dictionary<ChunkConformationType, int>(32);
            }
            public string ChunkName;
            public string[] ChunkClass;
            public uint MaxGeneration;

            public int CommandsProcessedAmount;

            public int GroupStackAmount;
            public int GroupSetAmount;
            public int GroupSwitchAmount;
            public int ChunkControllerAmount;
            public int CollisionDependentAmount;
            public int DimensionsIgnorantAmount;
            public int SuppressionAmount;
            public int SuppressedByAmount;
            public int InductionAmount;
            public int InducedByAmount;
            public int HiddenAmount;
            public int ConnectorAmount;
            public int TagAmount;
            public int GenerationAttributeAmount;
            public int IgnoreAddColliderAmount;
            public Dictionary<ChunkConformationType,int>  ConformationType;
        }

        public static GameObject Cook(GameObject semifinishedEnt, ChunkImportInformation chunkImportInformation)
        {
            Debug.Log($"Cooking entity: {semifinishedEnt}");

            ExecuteFbxCommands(semifinishedEnt, chunkImportInformation);

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

        private static void ApplyMaterials(GameObject chunk)
        {
            var colorAtlas = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/ColorSchemes/ColorScheme.mat");
            Assert.IsNotNull(colorAtlas);

            var renders = chunk.GetComponentsInChildren<Renderer>();

            foreach (var render in renders)
            {
                render.material = colorAtlas;
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
                baseComponent.InfluenceGroup = baseComponent.transform.GetComponentInParent<Group>(); 
            }

            chunkController.Validate();
        }
    }
}