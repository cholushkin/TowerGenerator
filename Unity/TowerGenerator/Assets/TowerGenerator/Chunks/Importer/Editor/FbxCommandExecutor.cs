using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Plugins.Alg;
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    public static class FbxCommandExecutor
    {
        private class CommandRegistrationEntry
        {
            public delegate FbxCommandBase CommandCreator();
            public string Name;
            public CommandCreator Creator;
        }

        private static readonly CommandRegistrationEntry[] CmdFactory =
        {
            new(){ Name = "ChunkController", Creator = ()=> new FbxCommandChunkController("ChunkController")},
            new(){ Name = "Generation", Creator = ()=> new FbxCommandChunkGeneration("Generation")},
            new(){ Name = "CollisionDependent", Creator = ()=> new FbxCommandCollisionDependent("CollisionDependent")},
            new(){ Name = "Connector", Creator = ()=> new FbxCommandConnector("Connector")},
            new(){ Name = "DimensionsIgnorant", Creator = ()=> new FbxCommandDimensionsIgnorant("DimensionsIgnorant")},
            new(){ Name = "GeneratorConnector", Creator = ()=> new FbxCommandGeneratorConnector("GeneratorConnector")},
            new(){ Name = "GroupSet", Creator = ()=> new FbxCommandGroupSet("GroupSet")},
            new(){ Name = "GroupStack", Creator = ()=> new FbxCommandGroupStack("GroupStack")},
            new(){ Name = "GroupSwitch", Creator = ()=> new FbxCommandGroupSwitch("GroupSwitch")},
            new(){ Name = "Hidden", Creator = ()=> new FbxCommandHidden("Hidden")},
            new(){ Name = "IgnoreAddCollider", Creator = ()=> new FbxCommandIgnoreAddCollider("IgnoreAddCollider")},
            new(){ Name = "InducedBy", Creator = ()=> new FbxCommandInducedBy("InducedBy")},
            new(){ Name = "Induction", Creator = ()=> new FbxCommandInduction("Induction")},
            new(){ Name = "Set", Creator = ()=> new FbxCommandSet("Set")},
            new(){ Name = "SuppressedBy", Creator = ()=> new FbxCommandSuppressedBy("SuppressedBy")},
            new(){ Name = "Suppression", Creator = ()=> new FbxCommandSuppression("Suppression")},
            new(){ Name = "Tag", Creator = ()=> new FbxCommandTag("Tag")},
        };

        public static void Execute(FbxProps fromFbxProps, GameObject gameObject,
            ChunkCooker.ChunkImportInformation chunkImportInformation)
        {
            Assert.IsNotNull(fromFbxProps);
            Assert.IsNotNull(gameObject);
            Assert.IsNotNull(fromFbxProps.Properties, $"empty props on {gameObject}");

            var commands = new List<FbxCommandBase>(fromFbxProps.Properties.Count);

            // parse commands
            foreach (var property in fromFbxProps.Properties)
            {
                string fbxCmdName = ParseFbxCommand(property);
                string fbxParameters = property.Value;

                var cmdCreator = CmdFactory.FirstOrDefault(x => x.Name == fbxCmdName);
                if (cmdCreator == null)
                {
                    Debug.LogError(
                        $"Unable to find cmd '{fbxCmdName}', fbx prop name = {property.Value}; fbx prop value = {property.Name}; object = '{gameObject.transform.GetDebugName()}' ");
                    break;
                }

                var cmd = cmdCreator.Creator();
                cmd.ParseParameters(fbxParameters, gameObject);
                cmd.SetRawInputFromFbx(fbxCmdName, fbxParameters);
                commands.Add(cmd);
                chunkImportInformation.CommandsProcessedAmount++;
            }

            // execute commands by their priorities
            foreach (var cmd in commands.OrderBy(c => c.GetExecutionPriority()))
            {
                Debug.Log($"Executing {cmd.RawInputFromFbx} on {gameObject.transform.GetDebugName()}");
                cmd.Execute(gameObject, chunkImportInformation);
            }
        }

        // note: fbx command could end with digit due to fbx props naming traits
        private static string ParseFbxCommand(FbxProps.Property property)
        {
            return Regex.Replace(property.Name, @"\d*$", "");
        }
    }
}