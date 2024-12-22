#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GameLib.Alg;
using TowerGenerator.ChunkImporter;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    [InitializeOnLoad]
    public static class FbxCommandExecutor
    {
        public class CommandRegistrationEntry
        {
            public delegate FbxCommandBase CommandCreator();
            public string Name;
            public CommandCreator Creator;
        }

        private static List<CommandRegistrationEntry> _commandsRegistered;

        static FbxCommandExecutor()
        {
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "ChunkController", Creator = () => new FbxCommandChunkController("ChunkController", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "Generation", Creator = () => new FbxCommandChunkGeneration("Generation", 0) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "CollisionDependent", Creator = () => new FbxCommandCollisionDependent("CollisionDependent", 10)});
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "Connector", Creator = () => new FbxCommandConnector("Connector", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "DimensionsIgnorant", Creator = () => new FbxCommandDimensionsIgnorant("DimensionsIgnorant", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "GeneratorConnector", Creator = () => new FbxCommandGeneratorConnector("GeneratorConnector", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "GroupSet", Creator = () => new FbxCommandGroupSet("GroupSet", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "GroupStack", Creator = () => new FbxCommandGroupStack("GroupStack", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "GroupSwitch", Creator = () => new FbxCommandGroupSwitch("GroupSwitch", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "Hidden", Creator = () => new FbxCommandHidden("Hidden", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "IgnoreAddCollider", Creator = () => new FbxCommandIgnoreAddCollider("IgnoreAddCollider", 10) }); 
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "InducedBy", Creator = () => new FbxCommandInducedBy("InducedBy", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "Induction", Creator = () => new FbxCommandInduction("Induction", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "Set", Creator = () => new FbxCommandSet("Set", 0) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "SuppressedBy", Creator = () => new FbxCommandSuppressedBy("SuppressedBy", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "Suppression", Creator = () => new FbxCommandSuppression("Suppression", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "Tag", Creator = () => new FbxCommandTag("Tag", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "Material", Creator = () => new FbxCommandMaterial("Material", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "Collider", Creator = () => new FbxCommandCollider("Collider", 10) });
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "IgnoreGroupItem", Creator = () => new FbxCommandIgnoreGroupItem("IgnoreGroupItem", 11) }); 
            RegisterFbxCommand(new CommandRegistrationEntry { Name = "Meta", Creator = () => new FbxCommandMeta("Meta", 12) });
        }

        public static void RegisterFbxCommand(CommandRegistrationEntry entry)
        {
            if (_commandsRegistered == null)
                _commandsRegistered = new List<CommandRegistrationEntry>();

            Assert.IsFalse(_commandsRegistered.Contains(entry), $"already registered {entry.Name}");
            _commandsRegistered.Add(entry);
        }

        public static void ParseFbxProps(FbxProps fromFbxProps, List<(GameObject, FbxCommandBase)> allCommands, ChunkImportState importState)
        {
            Assert.IsNotNull(fromFbxProps);
            Assert.IsNotNull(fromFbxProps.gameObject);
            Assert.IsNotNull(fromFbxProps.Properties, $"empty props on {fromFbxProps.gameObject}");
            
            // Parse FBX props
            foreach (var property in fromFbxProps.Properties)
            {
                string fbxCmdName = ParseFbxCommand(property);
                string fbxParameters = property.Value;

                var cmdCreator = _commandsRegistered.FirstOrDefault(x => x.Name == fbxCmdName);
                if (cmdCreator == null)
                {
                    Debug.LogError(
                        $"Unable to find cmd '{fbxCmdName}', fbx prop name = {property.Value}; fbx prop value = {property.Name}; object = '{fromFbxProps.gameObject.transform.GetDebugName()}' ");
                    break;
                }

                var cmd = cmdCreator.Creator();
                cmd.SetRawInputFromFbx(fbxCmdName, fbxParameters);
                Debug.Log($"Parsing {cmd.RawInputFromFbx} on {fromFbxProps.gameObject.transform.GetDebugName()}");
                cmd.ParseParameters(fbxParameters, fromFbxProps.gameObject);
                allCommands.Add((fromFbxProps.gameObject, cmd));
            }
            fromFbxProps.gameObject.RemoveComponent<FbxProps>();
        }

        public static void ExecuteCommands(List<(GameObject, FbxCommandBase)> commands, ChunkImportState importState)
        {
            // Execute commands by their priorities
            foreach (var goCmd in commands.OrderBy(c => c.Item2.GetExecutionPriority()))
            {
                var cmd = goCmd.Item2;
                var gObject = goCmd.Item1;
                Debug.Log($"Executing {cmd.RawInputFromFbx} on {gObject.transform.GetDebugName()}");
                cmd.Execute(gObject, importState);
                importState.CommandsProcessedAmount++;
            }
        }

        // Note: fbx command could end with digit due to fbx props naming traits
        private static string ParseFbxCommand(FbxProps.Property property)
        {
            return Regex.Replace(property.Name, @"\d*$", "");
        }
    }
}
#endif