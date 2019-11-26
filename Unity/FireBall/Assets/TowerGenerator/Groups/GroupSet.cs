using System;
using System.Collections.Generic;
using System.Linq;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    // Randomly enables some options from the group
    public class GroupSet : Group
    {
        public int MinObjectsSelected; // default 0
        public int MaxObjectsSelected; // default transform.childCount
        public int ItemsSelectedAmount;

        public override void Configure(Transform entityRoot, List<FbxProps.ScriptToAdd.ScriptProperty> scriptProperties)
        {
            base.Configure(entityRoot,scriptProperties);

            // get domain properties
            var propMinObjectsSelected = scriptProperties.FirstOrDefault(x => x.PropName == "MinObjectsSelected");
            var propMaxObjectsSelected = scriptProperties.FirstOrDefault(x => x.PropName == "MaxObjectsSelected");


            if (propMaxObjectsSelected != null)
                MaxObjectsSelected = GetMaxObjectsSelectedFromString(propMaxObjectsSelected.PropValue);
            else
                MaxObjectsSelected = GetItemsCount();

            if (propMinObjectsSelected != null)
                MinObjectsSelected = GetMinObjectsSelectedFromString(propMinObjectsSelected.PropValue);
            else
                MinObjectsSelected = 0;
        }

        public override int GetNumberOfPermutations()
        {
            return (int)Math.Pow(2, GetItemsCount());
        }

        private int GetMaxObjectsSelectedFromString(string propValue)
        {
            if (propValue.ToLower() == "all")
                return GetItemsCount();
            var maxObjectsSelectedParsed = Int32.Parse(propValue);
            var clamped= Mathf.Clamp(maxObjectsSelectedParsed,0, GetItemsCount());
            if (clamped != maxObjectsSelectedParsed)
                Debug.LogWarning($"Clamping happened {maxObjectsSelectedParsed} -> {clamped}");
            return clamped;
        }

        private int GetMinObjectsSelectedFromString(string propValue)
        {
            var minObjectsSelectedParsed = Int32.Parse(propValue);
            var clamped = Mathf.Clamp(minObjectsSelectedParsed, 0, MaxObjectsSelected);
            if (clamped != minObjectsSelectedParsed)
                Debug.LogWarning($"Clamping happened {minObjectsSelectedParsed} -> {clamped}");
            return clamped;
        }
        
        public override void DoRndChoice(ref RandomHelper rnd)
        {
            DisableItems();
            ItemsSelectedAmount = rnd.FromRangeIntInclusive(MinObjectsSelected, MaxObjectsSelected);
            int[] itemsIndexes = new int[GetItemsCount()];
            for (int i = 0; i < GetItemsCount(); ++i)
                itemsIndexes[i] = i;
            var choices = rnd.FromArray(itemsIndexes, ItemsSelectedAmount);
            for (int i = 0; i < ItemsSelectedAmount; ++i)
            {
                transform.GetChild(choices[i]).gameObject.SetActive(true);
            }
        }

        // assume that all items are sorted ascending 
        // pick minimal amount of options with minor indexes
        public override void DoRndMinimalChoice(ref RandomHelper rnd)
        {
            DisableItems();
            ItemsSelectedAmount = MinObjectsSelected;
            for (int i = 0; i < ItemsSelectedAmount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
