using TowerGenerator.ChunkImporter;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator.FbxCommands
{
    public class FbxCommandMaterial : FbxCommandBase
    {
        public string MaterialName;

        public FbxCommandMaterial(string fbxCommandName) : base(fbxCommandName)
        {
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsFalse(string.IsNullOrWhiteSpace(parameters));
            MaterialName = parameters.Trim();
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
        {
            var materialToApply = Resources.Load<Material>($"{MaterialName}");
            Assert.IsNotNull(materialToApply, $"Can't load material {MaterialName}.mat");

            // todo: apply recursively as an options

            var renderer = gameObject.GetComponent<Renderer>();
            renderer.sharedMaterial = materialToApply;
            renderer.receiveShadows = false;
        }
    }
}
