using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerGenerator
{
    public class FbxProps : MonoBehaviour
    {
        public class ScriptToAdd
        {
            public class ScriptProperty
            {
                public string PropName;
                public object PropValue;
            }

            public void AddProp(string propName, object value)
            {
                if(ScriptProperties == null)
                    ScriptProperties = new List<ScriptProperty>();
                ScriptProperties.Add( new ScriptProperty{PropName = propName, PropValue = value});
            }

            public string ScriptName;
            public List<ScriptProperty> ScriptProperties;
        }

        public List<ScriptToAdd> ScriptsToAdd;

        public void AddProp(string scriptName, string propName, object value)
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
    }
}

