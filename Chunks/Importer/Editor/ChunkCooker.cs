#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerGenerator.FbxCommands;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace TowerGenerator.ChunkImporter
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
        public string MetaType;
        public uint Generation;
        public TagSet ChunkTagSet;
        public ChunkControllerBase.ChunkController ChunkControllerType;
        public readonly ChunkImportSource ImportSource;
        private Dictionary<string, int> _stateCounters = new();

        
        public void Set(string keyName, int newValue)
        {
            _stateCounters[keyName] = newValue;
        }
        public void Inc(string keyName)
        {
            if (_stateCounters.ContainsKey(keyName))
                _stateCounters[keyName]++;
            else
                _stateCounters[keyName] = 1;
        }

        public int Get(string keyName)
        {
            return _stateCounters.TryGetValue(keyName, out int value) ? value : 0;
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder(JsonUtility.ToJson(this, true))
                .AppendLine()
                .AppendLine("- StateCounters")
                .AppendJoin("\n", _stateCounters.Select(kvp => $"  - {kvp.Key}: {kvp.Value}"));
            return sb.ToString();
        }
    }
    
    public interface IChunkCooker
    {
        IEnumerator Cook(ChunkImportSource importSource, GameObject semifinishedEnt, ChunkImportState chunkImportInformation);
    }

    public class ChunkCookerDefault : IChunkCooker
    {
        public virtual IEnumerator Cook(ChunkImportSource importSource, GameObject semifinishedEnt, ChunkImportState chunkImportInformation)
        {
            Debug.Log($"Cooking entity: {semifinishedEnt}");

            yield return EditorCoroutineUtility.StartCoroutineOwnerless(ExecuteFbxCommands(semifinishedEnt, chunkImportInformation));

            if (importSource.AddColliders)
                ApplyColliders(semifinishedEnt, chunkImportInformation);

            if (importSource.ApplyMaterials)
                ApplyMaterials(semifinishedEnt, chunkImportInformation);
            
            ApplyShadowsSettings(semifinishedEnt, importSource.CastShadows, chunkImportInformation);

            yield return null;

            ConfigureChunkController(semifinishedEnt, chunkImportInformation); // tree
        }

        protected virtual IEnumerator ExecuteFbxCommands(GameObject semifinishedEnt, ChunkImportState chunkImportInformation)
        {
            var fbxProps = semifinishedEnt.GetComponentsInChildren<FbxProps>(true);
            
            var allCommands = new List<(GameObject, FbxCommandBase)>(fbxProps.Length);

            // Parse all fbxProps
            foreach (var props in fbxProps)
                FbxCommandExecutor.ParseFbxProps(props, allCommands, chunkImportInformation);

            // Execute all commands
            FbxCommandExecutor.ExecuteCommands(allCommands, chunkImportInformation);
            yield return null;
        }

        protected virtual void ApplyColliders(GameObject semifinishedEnt, ChunkImportState chunkImportInformation)
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
                        chunkImportInformation.Inc("CollidersAppliedAmmount");
                    }
                }
            }
        }

        protected virtual void ApplyMaterials(GameObject chunk, ChunkImportState chunkImportInformation)
        {
            var defaultMaterial = chunkImportInformation.ImportSource.DefaultMaterial;
            Assert.IsNotNull(defaultMaterial, ChunkImporterHelper.AddStateInformation("defaultMaterial is not assigned", chunkImportInformation));

            var renders = chunk.GetComponentsInChildren<Renderer>();
            foreach (var render in renders)
                render.material = defaultMaterial;
        }
        
        protected virtual void ApplyShadowsSettings(GameObject chunk, bool castShadows, ChunkImportState chunkImportInformation)
        {
            var renders = chunk.GetComponentsInChildren<Renderer>();
            foreach (var render in renders)
            {
                render.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
                render.receiveShadows = castShadows;
            }
        }

        protected virtual void ConfigureChunkController(GameObject chunkSemicooked, ChunkImportState chunkImportInformation)
        {
            var chunkController = chunkSemicooked.GetComponent<ChunkControllerBase>();
            Assert.IsNotNull(chunkController, ChunkImporterHelper.AddStateInformation("There is no ChunkControllerBase on chunk. Probably missing ChunkController prop on chunk root in fbx", chunkImportInformation));

            var baseComponents = chunkSemicooked.GetComponentsInChildren<BaseComponent>(true);
            foreach (var baseComponent in baseComponents)
                baseComponent.ChunkController = chunkController;

            chunkController.Init();
        }
    }
}
#endif
