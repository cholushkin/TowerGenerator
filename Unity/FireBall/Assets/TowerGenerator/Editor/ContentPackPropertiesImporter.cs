using System;
using Assets.Plugins.Alg;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class ContentPackPropertiesImporter : AssetPostprocessor
    {
        // Due to a bug in unity that doesn't allow attach more than one script to game object
        // warning appears in the console - "Identifier uniqueness violation: 'xxx'. Scripted Importers do not guarantee that subsequent imports of this asset will properly re-link to these targets."
        // we just attach FbxProps script to objects and import objects using another technique

        // One of the property is "AddScript" with name of scripts that user wants to add, separated by coma "NameOfScript1, NameOfScript2" 
        // Other properties are for the parameters of each script
        // If there is only one script to add than all property's names are just plain
        // Otherwise the prefix is placed before the name of the property
        public void OnPostprocessGameObjectWithUserProperties(GameObject gObj, string[] names, System.Object[] values)
        {
            var fbxProps = gObj.AddComponent<FbxProps>();
            string[] scriptNames = null;

            // get scripts to add
            var index = Array.FindIndex(names, x => x == PropertyParserHelper.PropNameAddScript);
            if (index == -1)
            {
                Debug.LogError($"There must be property 'AddScript' on {gObj.transform.GetDebugName()}");
                return;
            }

            string sNames = (string)values[index];
            scriptNames = sNames.Replace(" ", String.Empty).Split(',');

            foreach (var scriptName in scriptNames)
                fbxProps.AddScript(scriptName);

            for (int i = 0; i < names.Length; ++i)
            {
                if(names[i] == PropertyParserHelper.PropNameAddScript)
                    continue;
                // pass properties to appropriate script
                var prefixAndPropName = names[i].Split('.');
                string scriptName;
                string propName;
                Assert.IsTrue(prefixAndPropName.Length == 2 || prefixAndPropName.Length == 1);
                if (prefixAndPropName.Length == 2) // has script name specificator in the prop name
                {
                    scriptName = prefixAndPropName[0];
                    propName = prefixAndPropName[1];
                    Assert.IsTrue(scriptName.Contains(scriptName));
                }
                else // just name of prop
                {
                    scriptName = scriptNames[0];
                    propName = prefixAndPropName[0];
                }
                fbxProps.AddProp(scriptName, propName, Convert.ToString(values[i]));
            }
        }

        public override int GetPostprocessOrder()
        {
            return 0;
        }
    }
}
