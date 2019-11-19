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


        // First property: "AddScript" : "NameOfScript1, NameOfScript2" 
        // After there are properties for the parameters of each script
        // if there is only one script to add than all property's names are just plain
        // Otherwise the prefix is placed before then name of the property
        public void OnPostprocessGameObjectWithUserProperties(GameObject gObj, string[] names, System.Object[] values)
        {
            var fbxProps = gObj.AddComponent<FbxProps>();
            string[] scriptNames = null;

            // get scripts to add
            var index = Array.FindIndex(names, x => x == "AddScript");
            if (index == -1)
            {
                Debug.LogError($"There must be property 'AddScript'");
                Debug.LogError($"{gObj.transform.GetDebugName()}");
                return;
            }

            string sNames = (string)values[index];
            scriptNames = sNames.Replace(" ", String.Empty).Split(',');

            foreach (var scriptName in scriptNames)
                fbxProps.AddScript(scriptName);

            for (int i = 0; i < names.Length; ++i)
            {
                if(names[i] == "AddScript")
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
                fbxProps.AddProp(scriptName, propName, values[i]);
            }
        }

        //private List<Tuple<GameObject, string[], System.Object[]>> _props;

        //public void OnPreprocessModel()
        //{
        //    _props = new List<Tuple<GameObject, string[], object[]>>(128);
        //}

        //public void OnPostprocessMeshHierarchy(GameObject gObj) //The ModelImporter calls this function for every root transform hierarchy in the source model file.
        //{
        //    gObj.transform.ForEachChildrenRecursive(AddComponents);
        //}

        //public void OnPostprocessGameObjectWithUserProperties(GameObject gObj, string[] names, System.Object[] values)
        //{
        //    _props.Add(Tuple.Create(gObj,names,values));
        //}

        //public void OnPostprocessModel(GameObject contentPack)
        //{
        //    // note: at this point the hierarchy exists already, all scripts are attached
        //    //Debug.Log("passC " + contentPack.transform.GetDebugName());

        //    // todo: support properties for multyentities on one object
        //    if(_props.Count == 0)
        //        Debug.LogWarning($"no props at all, maybe you forgot to export 'Custom Properties'");
        //    foreach (var tuple in _props)
        //    {
        //        var gObj = tuple.Item1;
        //        var names = tuple.Item2;
        //        var values = tuple.Item3;

        //        var propImporter = gObj.GetComponent<BasePropImporter>();
        //        Assert.IsNotNull(propImporter, $"{gObj.transform.GetDebugName()} has no BasePropImporter on it but trying to set properties");

        //        // override default values with user specified values
        //        for (int i = 0; i < names.Length; i++)
        //        {
        //            string propertyName = names[i];
        //            object propertyValue = values[i];
        //            var isOK = propImporter.SetProp(propertyName, propertyValue);
        //            Assert.IsTrue(isOK, $"Couldn't set prop '{propertyName}' to value '{propertyValue}'. Object is {gObj.transform.GetDebugName()}");
        //        }
        //    }
        //}

        //private void AddComponents(Transform transform)
        //{
        //    // by name
        //    if (transform.name.StartsWith("Connectors"))
        //    {
        //        transform.gameObject.AddComponent<Connectors>();
        //    }

        //    // by script class
        //    if (transform.name.Contains("<Layer>"))
        //    {
        //        transform.gameObject.AddComponent<Layer>().SetDefaultValues();
        //    }
        //    else if (transform.name.Contains("<ChunkStd>"))
        //    {
        //        transform.gameObject.AddComponent<ChunkStd>().SetDefaultValues();
        //    }
        //    else if (transform.name.Contains("<ChunkRoofPeek>"))
        //    {
        //        transform.gameObject.AddComponent<ChunkRoofPeek>().SetDefaultValues();
        //    }
        //    else if (transform.name.Contains("<GroupStack>"))
        //    {
        //        transform.gameObject.AddComponent<GroupStack>().SetDefaultValues();
        //    }
        //    else if (transform.name.Contains("<GroupUser>"))
        //    {
        //        transform.gameObject.AddComponent<GroupUser>().SetDefaultValues();
        //    }
        //    else if (transform.name.Contains("<GroupRndSwitch>"))
        //    {
        //        transform.gameObject.AddComponent<GroupRndSwitch>().SetDefaultValues();
        //    }
        //    else if (transform.name.Contains("<GroupRndSet>"))
        //    {
        //        transform.gameObject.AddComponent<GroupRndSet>().SetDefaultValues();
        //    }
        //}
    }
}
