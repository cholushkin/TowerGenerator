using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerGenerator.ChunkImporter
{
    public static class PropertyParserHelper
    {
        // Property names
        public const string PropNameMinObjectsSelected = "MinObjectsSelected";
        public const string PropNameMaxObjectsSelected = "MaxObjectsSelected";
        public const string PropNameAddScript = "AddScript";
        public const string PropNamePropagatedTo = "PropagatedTo";
        public const string PropNameHost = "Host";
        public const string PropNameCollisionCheck = "CollisionCheck";
#if UNITY_EDITOR
        public static bool CheckPropNames(List<FbxProps.ScriptToAdd.ScriptProperty> scriptProperties, params string[] propNamesPossible)
        {
            foreach (var sp in scriptProperties)
                if (!propNamesPossible.Contains(sp.PropName))
                {
                    Debug.Log($"Bad property name: {sp.PropName}");
                    return false;
                }

            return true;
        }
#endif
        public static bool ParseFloat(string propValue, out float result)
        {
            return Single.TryParse(propValue, out result);
        }

        public static bool ParseInt(string propValue, out int result)
        {
            return Int32.TryParse(propValue, out result);
        }
    }
}

