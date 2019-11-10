using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TowerGenerator
{

    public class ContentPackProps : AssetPostprocessor
    {
        // entities
        private const string Connector = "<Connector>";
        private const string ChunkStd = "<ChunkStd>";
        private const string ChunkRoofPeak = "<ChunkRoofPeek>";
        private const string ChunkIsland = "<ChunkIsland>";
        private const string ContentCreatureHumanoid = "<ContentCreatureHumanoid>";

        private const string Group = "<Group>";

        // group properties
        private const string GroupType = "GroupType";

        private const string SetGroupType = "SetGroup";
        private const string StackGroupType = "StackGroup";
        private const string SwitchGroupType = "SwitchGroup";
        private const string ConnectorsGroupType = "ConnectorsGroup";
        private const string LayersGroupType = "LayersGroup";

        private const string MaxLayerIndexPropagated = "MaxLayerIndexPropagated";
        private const string MinObjectsSelected = "MinObjectsSelected";
        private const string MaxObjectsSelected = "MaxObjectsSelected";


        void OnPostprocessMeshHierarchy(GameObject gObj)
        {
            TraverseHierarchy(gObj.transform);
        }

        private void TraverseHierarchy(Transform from) // todo: use alg
        {
            AddComponents(from);
            foreach (Transform t in from)
                TraverseHierarchy(t);
        }

        private void AddComponents(Transform transform)
        {
            if (transform.name.Contains("<ChunkStd>"))
            {
                transform.gameObject.AddComponent<ChunkStd>();
            }
            else if (transform.name.Contains("<GroupStack>"))
            {
                transform.gameObject.AddComponent<GroupStack>();
            }
            else if (transform.name.Contains("<GroupUser>"))
            {
                transform.gameObject.AddComponent<GroupUser>();
            }
            else if (transform.name.Contains("<GroupRndSwitch>"))
            {
                transform.gameObject.AddComponent<GroupRndSwitch>();
            }
            else if (transform.name.Contains("<GroupRndSet>"))
            {
                transform.gameObject.AddComponent<GroupRndSet>();
            }
        }


        void OnPostprocessGameObjectWithUserProperties(GameObject gObj, string[] names, System.Object[] values)
        {
            for (int i = 0; i < names.Length; i++)
            {
                string propertyName = names[i];
                object propertyValue = values[i];


                //Debug.Log($"Object: {gObj}, hash {gObj.GetHashCode()}, Propname: {propertyName}, Value: {propertyValue}");
                //go.AddComponent<ChunkStd>();
            }
        }
    }


}
