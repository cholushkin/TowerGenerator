﻿using GameLib.Random;
using UnityEngine;


namespace TowerGenerator
{
    public abstract class Group : MonoBehaviour, INodeValidation
    {
        public int PropagatedTo; // by default it is propagated only to current layer (-1 ignoring propagation)
        public GroupStack Host; // host group to propagate to

        #region from IValidation

        public abstract bool IsValid();

        public virtual void Fix()
        {
        }
        #endregion


//        public virtual void Configure(Transform entityRoot, List<FbxProps.ScriptToAdd.ScriptProperty> scriptProperties)
//        {
//#if UNITY_EDITOR
//            if (!ChunkImporterHelper.CheckPropNames(
//                scriptProperties,
//                ChunkImporterHelper.PropNamePropagatedTo,
//                ChunkImporterHelper.PropNameHost,
//                ChunkImporterHelper.PropNameMaxObjectsSelected,
//                ChunkImporterHelper.PropNameMinObjectsSelected
//            ))
//                Debug.LogError($"Bad property name on '{transform.GetDebugName()}'");
//#endif
//            // get domain properties
//            var propPropagatedTo = scriptProperties.FirstOrDefault(x => x.PropName == ChunkImporterHelper.PropNamePropagatedTo);
//            var propHost = scriptProperties.FirstOrDefault(x => x.PropName == ChunkImporterHelper.PropNameHost);

//            // ----- get host group
//            if (propHost == null)
//            {
//                // get default host
//                var pointer = transform;
//                while (pointer != entityRoot)
//                {
//                    pointer = pointer.parent;
//                    var hostCandidate = pointer.GetComponent<GroupStack>();
//                    if (hostCandidate != null)
//                    {
//                        Host = hostCandidate;
//                        break;
//                    }
//                }
//            }
//            else
//            {
//                // get host by name
//                Host = entityRoot.GetComponentsInChildren<GroupStack>(true).FirstOrDefault(x => x.name == propHost.PropValue);
//                Assert.IsNotNull(Host, $"requesting name: {propHost.PropValue}");
//            }

//            // ----- get propagated to
//            if (propPropagatedTo == null)
//            {
//                // set default propagation
//                if (Host == null)
//                    PropagatedTo = -1;
//                else
//                {
//                    var pointer = transform;
//                    PropagatedTo = -1;
//                    while (pointer != Host && pointer != entityRoot)
//                    {
//                        pointer = pointer.parent;
//                        for (int i = 0; i < Host.GetItemsCount(); i++)
//                            if (Host.transform.GetChild(i) == pointer)
//                            {
//                                PropagatedTo = i;
//                                break;
//                            }
//                    }
//                }
//            }
//            else
//            {
//                if (Host == null)
//                    Debug.LogError("no host");
//                else
//                    // set propagation by index
//                    PropagatedTo = GetPropagationIndexFromString(propPropagatedTo.PropValue);
//            }
//        }

        public abstract int GetNumberOfPermutations();


        public abstract void DoChoice(params int[] index);
        public abstract void DoRndChoice(ref RandomHelper rnd);
        public abstract void DoRndMinimalChoice(ref RandomHelper rnd);

        //protected int GetPropagationIndexFromString(string propValue)
        //{
        //    var propIndex = -1;
        //    if (Host == null)
        //    {
        //        Debug.LogError($"no host for propagation index {propValue}");
        //        return propIndex;
        //    }

        //    if (propValue.ToLower() == "all")
        //    {
        //        propIndex = Host.GetItemsCount() - 1;
        //        return propIndex;
        //    }


        //    if (!ChunkImporterHelper.ParseInt(propValue, out propIndex))
        //        Debug.LogError($"Parsing int error for value {propValue}");
        //    var clampedPropIndex = Mathf.Clamp(propIndex, -1, Host.GetItemsCount() - 1);
        //    if (propIndex != clampedPropIndex)
        //        Debug.LogWarning($"Claming happened {propIndex} -> {clampedPropIndex}");
        //    return clampedPropIndex;
        //}

        public void DisableItems()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        public virtual int GetItemsCount()
        {
            return transform.childCount;
        }
    }
}