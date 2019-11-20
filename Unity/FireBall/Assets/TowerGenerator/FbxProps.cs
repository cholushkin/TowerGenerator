using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerGenerator
{
    public class FbxProps : MonoBehaviour
    {
        [Serializable]
        public class ScriptToAdd
        {
            [Serializable]
            public class ScriptProperty
            {
                public string PropName;
                public string PropValue;
            }

            public void AddProp(string propName, string value)
            {
                if(ScriptProperties == null)
                    ScriptProperties = new List<ScriptProperty>();
                ScriptProperties.Add( new ScriptProperty{PropName = propName, PropValue = value});
            }

            public string ScriptName;
            public List<ScriptProperty> ScriptProperties;
        }

        public List<ScriptToAdd> ScriptsToAdd;

        public void AddProp(string scriptName, string propName, string value)
        {
            if (ScriptsToAdd == null)
                ScriptsToAdd = new List<ScriptToAdd>(2);
            var scriptToAdd = ScriptsToAdd.FirstOrDefault(x => x.ScriptName == scriptName);
            if (scriptToAdd == null)
            {
                scriptToAdd = new ScriptToAdd {ScriptName = scriptName};
                ScriptsToAdd.Add(scriptToAdd);
            }
            scriptToAdd.AddProp(propName, value);
        }

        public void AddScript(string scriptName)
        {
            if (ScriptsToAdd == null)
                ScriptsToAdd = new List<ScriptToAdd>(2);
            if (ScriptsToAdd.FirstOrDefault(x => x.ScriptName == scriptName) != null)
            {
                Debug.LogError("Multiple script adding");
                return;
            }
            ScriptsToAdd.Add(new ScriptToAdd { ScriptName = scriptName });
        }

        [ContextMenu("DbgPrint")]
        void DbgPrint()
        {
            foreach (var scriptToAdd in ScriptsToAdd)
            {
                Debug.Log(JsonUtility.ToJson(scriptToAdd, true));
            }
            
        }
    }
}

