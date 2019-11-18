using System;
using System.Collections.Generic;
using Assets.Plugins.Alg;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{

    public class ContentPackProps : AssetPostprocessor
    {
        private List<Tuple<GameObject, string[], System.Object[]>> _props;

        public void OnPreprocessModel()
        {
            _props = new List<Tuple<GameObject, string[], object[]>>(128);
        }

        public void OnPostprocessMeshHierarchy(GameObject gObj) //The ModelImporter calls this function for every root transform hierarchy in the source model file.
        {
            gObj.transform.ForEachChildrenRecursive(AddComponents);
        }

        public void OnPostprocessGameObjectWithUserProperties(GameObject gObj, string[] names, System.Object[] values)
        {
            //Debug.Log("passB " + gObj.transform.GetDebugName());
            _props.Add(Tuple.Create(gObj,names,values));
        }

        public void OnPostprocessModel(GameObject contentPack)
        {
            // note: at this point the hierarchy exists already, all scripts are attached
            //Debug.Log("passC " + contentPack.transform.GetDebugName());

            // todo: support properties for multyentities on one object
            if(_props.Count == 0)
                Debug.LogWarning($"no props at all, maybe you forgot to export 'Custom Properties'");
            foreach (var tuple in _props)
            {
                var gObj = tuple.Item1;
                var names = tuple.Item2;
                var values = tuple.Item3;

                var propImporter = gObj.GetComponent<BasePropImporter>();
                Assert.IsNotNull(propImporter, $"{gObj.transform.GetDebugName()} has no BasePropImporter on it but trying to set properties");

                // override default values with user specified values
                for (int i = 0; i < names.Length; i++)
                {
                    string propertyName = names[i];
                    object propertyValue = values[i];
                    var isOK = propImporter.SetProp(propertyName, propertyValue);
                    Assert.IsTrue(isOK, $"Couldn't set prop '{propertyName}' to value '{propertyValue}'. Object is {gObj.transform.GetDebugName()}");
                }
            }
        }

        private void AddComponents(Transform transform)
        {
            // by name
            if (transform.name.StartsWith("Connectors"))
            {
                transform.gameObject.AddComponent<Connectors>();
            }

            // be script class
            if (transform.name.Contains("<Layer>"))
            {
                transform.gameObject.AddComponent<Layer>().SetDefaultValues();
            }
            else if (transform.name.Contains("<ChunkStd>"))
            {
                transform.gameObject.AddComponent<ChunkStd>().SetDefaultValues();
            }
            else if (transform.name.Contains("<ChunkRoofPeek>"))
            {
                transform.gameObject.AddComponent<ChunkRoofPeek>().SetDefaultValues();
            }
            else if (transform.name.Contains("<GroupStack>"))
            {
                transform.gameObject.AddComponent<GroupStack>().SetDefaultValues();
            }
            else if (transform.name.Contains("<GroupUser>"))
            {
                transform.gameObject.AddComponent<GroupUser>().SetDefaultValues();
            }
            else if (transform.name.Contains("<GroupRndSwitch>"))
            {
                transform.gameObject.AddComponent<GroupRndSwitch>().SetDefaultValues();
            }
            else if (transform.name.Contains("<GroupRndSet>"))
            {
                transform.gameObject.AddComponent<GroupRndSet>().SetDefaultValues();
            }
        }
    }
}
