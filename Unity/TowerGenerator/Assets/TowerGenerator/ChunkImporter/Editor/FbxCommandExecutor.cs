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
        private class CommandItem
        {
            public delegate FbxCommandBase CommandCreator();
            public string Name;
            public CommandCreator Creator;
            public override string ToString()
            {
                return base.ToString();
            }
        }

        private static readonly CommandItem[] CmdFactory =
        {
            // Groups
            new CommandItem{ Name = "GroupStack", Creator = ()=> new FbxCommandChunkController()},
            new CommandItem{ Name = "GroupSet", Creator = ()=> new FbxCommandChunkGeneration()},
            new CommandItem{ Name = "GroupSwitch", Creator = ()=> new FbxCommandCollisionDependent()},
            new CommandItem{ Name = "ChunkController", Creator = ()=> new FbxCommandConnector()},
            new CommandItem{ Name = "Connector", Creator = ()=> new FbxCommandDimensionsIgnorant()},
            new CommandItem{ Name = "Tag", Creator = ()=> new FbxCommandSet()},
            new CommandItem{ Name = "CollisionDependent", Creator = ()=> new FbxCommandGroupStack()},
            new CommandItem{ Name = "DimensionsIgnorant", Creator = ()=> new FbxCommandGroupSwitch()},
            new CommandItem{ Name = "Suppression", Creator = ()=> new FbxCommandHidden()},
            new CommandItem{ Name = "SuppressedBy", Creator = ()=> new FbxCommandIgnoreAddCollider()},
            new CommandItem{ Name = "Induction", Creator = ()=> new FbxCommandInducedBy()},
            new CommandItem{ Name = "InducedBy", Creator = ()=> new FbxCommandInduction()},
            new CommandItem{ Name = "Hidden", Creator = ()=> new FbxCommandSet()},
            new CommandItem{ Name = "ClassName", Creator = ()=> new FbxCommandSuppressedBy()},
            new CommandItem{ Name = "Generation", Creator = ()=> new FbxCommandSuppression()},
            new CommandItem{ Name = "ComponentValue", Creator = ()=> new FbxCommandTag()},

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
                        $"Unable to find cmd '{fbxCmdName}', fbx prop name = {property.Value}; fbx prop value ={property.Name}; object = '{gameObject.transform.GetDebugName()}' ");
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