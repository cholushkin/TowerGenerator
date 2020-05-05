using System;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Plugins.Alg;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator.ChunkImporter
{
    // see all commands: https://docs.google.com/spreadsheets/d/1sefKKZGdllpTpHPTX2AZMRrZCT5wT7QiN8GhPCoiGW4/edit#gid=0

    public static class FbxCommand
    {
        #region Fbx commands hierarchy
        private abstract class FbxCommandBase
        {
            public class Parameter<T>
            {
                public string Name;
                public T Value;
            }

            public abstract string GetFbxCommandName();
            public abstract string GetPayloadCommandName();
            public abstract void ParseParameters(string parameters, GameObject gameObject);
            public abstract void Execute(GameObject gameObject);

            protected static float ConvertFloat01(string float01String)
            {
                float value = float.Parse(float01String);

                var clamped = Mathf.Clamp(value, 0.0f, 1.0f);
                if (Math.Abs(clamped - value) > 0.00001f)
                    Debug.LogWarning($"Clamping happened {value} -> {clamped}");
                return clamped;
            }

            protected static int ConvertInt(string intString)
            {
                int value = Int32.Parse(intString);
                return value;
            }

            protected static bool ConvertBool(string boolString)
            {
                bool value = bool.Parse(boolString);
                return value;
            }

            protected T ConvertEnum<T>(string enumString) where T : struct
            {
                try
                {
                    T res = (T)Enum.Parse(typeof(T), enumString);
                    if (!Enum.IsDefined(typeof(T), res))
                        return default(T);
                    return res;
                }
                catch
                {
                    return default(T);
                }
            }
        }

        private abstract class FbxCommandAddComponent : FbxCommandBase
        {
            public override string GetFbxCommandName()
            {
                return "AddComponent";
            }
        }

        private abstract class FbxCommandAddTag : FbxCommandBase
        {
            public override string GetFbxCommandName()
            {
                return "AddTag";
            }
        }

        private abstract class FbxCommandAddNodeAttribute : FbxCommandBase
        {
            public override string GetFbxCommandName()
            {
                return "AddNodeAttribute";
            }
        }
        #endregion

        #region Groups
        private class GroupStack : FbxCommandAddComponent
        {

            public override string GetPayloadCommandName()
            {
                return "GroupStack";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.GroupStack>();
            }
        }

        private class GroupSet : FbxCommandAddComponent
        {
            private Parameter<int> _minObjectsSelected;
            private Parameter<int> _maxObjectsSelected;

            public override string GetPayloadCommandName()
            {
                return "GroupSet";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                // set defaults parameters first
                _minObjectsSelected = new Parameter<int> { Name = "MinObjectsSelected", Value = 0 };
                _maxObjectsSelected = new Parameter<int> { Name = "MaxObjectsSelected", Value = gameObject.transform.childCount };

                if (string.IsNullOrWhiteSpace(parameters))
                    return;

                var actualParams = parameters.Split(',');
                Assert.IsTrue(actualParams.Length == 1 || actualParams.Length == 2);
                if (actualParams.Length >= 1)
                {
                    _minObjectsSelected.Value = ConvertInt(actualParams[0]);
                }
                if (actualParams.Length >= 2)
                {
                    _maxObjectsSelected.Value = ConvertInt(actualParams[1]);
                }
            }

            public override void Execute(GameObject gameObject)
            {
                var groupSetComp = gameObject.AddComponent<global::TowerGenerator.GroupSet>();
                groupSetComp.MaxObjectsSelected = _maxObjectsSelected.Value;
                groupSetComp.MinObjectsSelected = _minObjectsSelected.Value;
            }
        }

        private class GroupSwitch : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "GroupSwitch";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.GroupSwitch>();
            }
        }
        #endregion

        #region Chunk types
        private class ChunkTowerRoofPeekSegment : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "ChunkTowerRoofPeekSegment";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.ChunkTowerRoofPeekSegment>();
            }
        }

        private class ChunkTowerStandardSegment : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "ChunkTowerStandardSegment";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.ChunkTowerStandardSegment>();
            }
        }

        private class ChunkTowerIslandAndBasementSegment : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "ChunkTowerIslandAndBasementSegment";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.ChunkTowerIslandAndBasementSegment>();
            }
        }

        private class ChunkTowerFlyingIslandSegment : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "ChunkFlyingIslandSegment";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.ChunkTowerFlyingIslandSegment>();
            }
        }

        private class ChunkTowerSideEarSegment : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "ChunkTowerSideEarSegment";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.ChunkTowerSideEarSegment>();
            }
        }

        private class ChunkTowerBottomEarSegment : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "ChunkTowerBottomEarSegment";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.ChunkTowerBottomEarSegment>();
            }
        }

        private class ChunkTowerConnectorVertical : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "ChunkTowerConnectorVertical";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.ChunkTowerConnectorVertical>();
            }
        }

        private class ChunkTowerConnectorHorizontal : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "ChunkTowerConnectorHorizontal";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.ChunkTowerConnectorHorizontal>();
            }
        }
        #endregion

        #region Node attributes
        private class CollisionDependant : FbxCommandAddNodeAttribute
        {
            private Parameter<global::TowerGenerator.CollisionDependant.CollisionCheckMode> _collisionMode;
            public override string GetPayloadCommandName()
            {
                return "CollisionDependant";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                // set default values first
                _collisionMode = new Parameter<global::TowerGenerator.CollisionDependant.CollisionCheckMode> { Name = "CollisionMode", Value = global::TowerGenerator.CollisionDependant.CollisionCheckMode.MeshBased };

                if (string.IsNullOrWhiteSpace(parameters))
                    return;

                parameters = parameters.Trim();

                Assert.IsTrue(parameters.All(char.IsLetter));

                _collisionMode.Value = ConvertEnum<global::TowerGenerator.CollisionDependant.CollisionCheckMode>(parameters);
            }

            public override void Execute(GameObject gameObject)
            {
                var comp = gameObject.AddComponent<global::TowerGenerator.CollisionDependant>();
                comp.CollisionCheck = _collisionMode.Value;
            }
        }

        private class DimensionsIgnorant : FbxCommandAddNodeAttribute
        {
            public override string GetPayloadCommandName()
            {
                return "DimensionsIgnorant";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.GroupStack>();
            }

        }

        private class Suppression : FbxCommandAddNodeAttribute
        {
            private string[] _suppressionLabels;

            public override string GetPayloadCommandName()
            {
                return "Suppression";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));
                _suppressionLabels = parameters.Split(',');
            }

            public override void Execute(GameObject gameObject)
            {
                var comp = gameObject.AddComponent<global::TowerGenerator.Suppression>();
                comp.SuppressionLabels = _suppressionLabels;
            }
        }

        private class SuppressedBy : FbxCommandAddNodeAttribute
        {
            private string[] _suppressionLabels;
            public override string GetPayloadCommandName()
            {
                return "SuppressedBy";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));
                _suppressionLabels = parameters.Split(',');
            }

            public override void Execute(GameObject gameObject)
            {
                var comp = gameObject.AddComponent<global::TowerGenerator.SuppressedBy>();
                comp.SuppressionLabels = _suppressionLabels;
            }
        }

        private class Induction : FbxCommandAddNodeAttribute
        {
            private string[] _inductionLabels;
            public override string GetPayloadCommandName()
            {
                return "Induction";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));
                _inductionLabels = parameters.Split(',');
            }

            public override void Execute(GameObject gameObject)
            {
                var comp = gameObject.AddComponent<global::TowerGenerator.Induction>();
                comp.InductionLabels = _inductionLabels;
            }
        }

        private class InducedBy : FbxCommandAddNodeAttribute
        {
            private string[] _inductionLabels;
            public override string GetPayloadCommandName()
            {
                return "InducedBy";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));
                _inductionLabels = parameters.Split(',');
            }

            public override void Execute(GameObject gameObject)
            {
                var comp = gameObject.AddComponent<global::TowerGenerator.InducedBy>();
                comp.InductionLabels = _inductionLabels;
            }
        }
        #endregion

        #region Miscellaneous logic
        private class Connectors : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "Connectors";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject)
            {
                gameObject.AddComponent<global::TowerGenerator.Connectors>();
            }
        }

        private class AddTag : FbxCommandAddTag
        {
            private Parameter<string> _tagName;
            private Parameter<float> _tagValue;

            public override string GetPayloadCommandName()
            {
                return "Tag";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                // set defaults parameters first
                _tagName = new Parameter<string> { Name = "TagName", Value = "" };
                _tagValue = new Parameter<float> { Name = "TagValue", Value = gameObject.transform.childCount };

                Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));

                var actualParams = parameters.Split(',');
                Assert.IsTrue(actualParams.Length == 1 || actualParams.Length == 2);

                if (actualParams.Length >= 1)
                {
                    _tagName.Value = actualParams[0];
                }

                if (actualParams.Length >= 2)
                {
                    _tagValue.Value = ConvertFloat01(actualParams[1]);
                }
            }

            public override void Execute(GameObject gameObject)
            {
                var tagHolder = gameObject.GetComponent<TagHolder>();
                if (tagHolder == null)
                    tagHolder = gameObject.AddComponent<TagHolder>();

                tagHolder.TagSet.SetTag(_tagName.Value, _tagValue.Value);
            }
        }
        #endregion



        private static FbxCommandBase[] _fbxCommands =
        {
            // Groups
            new GroupStack(),
            new GroupSet(), 
            new GroupSwitch(),

            // Chunk types
            new ChunkTowerRoofPeekSegment(),
            new ChunkTowerStandardSegment(),
            new ChunkTowerIslandAndBasementSegment(),
            new ChunkTowerFlyingIslandSegment(),
            new ChunkTowerSideEarSegment(),
            new ChunkTowerBottomEarSegment(),
            new ChunkTowerConnectorVertical(),
            new ChunkTowerConnectorHorizontal(),

            // Node attributes
            new CollisionDependant(),
            new DimensionsIgnorant(),
            new Suppression(),
            new SuppressedBy(),
            new Induction(),
            new InducedBy(),

            // Miscellaneous logic
            new Connectors(),
            new AddTag()
        };

        public static void Execute(FbxProps fromFbxProps, GameObject gameObject)
        {
            Assert.IsNotNull(fromFbxProps);
            Assert.IsNotNull(gameObject);
            Assert.IsNotNull(fromFbxProps.Properties, $"empty props on {gameObject}");

            foreach (var property in fromFbxProps.Properties)
            {
                string fbxCmdName = ParseFbxCommand(property);
                string payloadCmd = ParsePayloadCommand(property);
                string payloadParameters = ParsePayloadParameters(property);

                var cmd = _fbxCommands.FirstOrDefault(x => x.GetFbxCommandName() == fbxCmdName && x.GetPayloadCommandName() == payloadCmd);
                if (cmd == null)
                    Debug.LogError($"Unable to find cmd '{fbxCmdName}', payloadCmd = '{payloadCmd}', payloadParameters = '{payloadParameters}', object = '{gameObject.transform.GetDebugName()}' ");
                cmd.ParseParameters(payloadParameters, gameObject);
                cmd.Execute(gameObject);
            }
        }

        private static string ParsePayloadCommand(FbxProps.Property property)
        {
            var match = Regex.Match(property.Value, @"^[^\( ]+");
            return match.Groups[0].Value;
        }

        // info: fbx command could end with digit due to fbx props naming
        private static string ParseFbxCommand(FbxProps.Property property)
        {
            return Regex.Replace(property.Name, @"\d*$", "");
        }

        // get string inside parentheses 
        private static string ParsePayloadParameters(FbxProps.Property property)
        {
            var matchGroups = Regex.Match(property.Value, @"\(([^\)]+)\)").Groups;
            return matchGroups[1].Value;
        }
    }
}
