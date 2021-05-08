using System;
using System.Collections.Generic;
using System.Linq;
using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    // Randomly enables some options from the group
    public class GroupSet : Group
    {
        public int MinObjectsSelected; // default 0
        public int MaxObjectsSelected; // default transform.childCount
        public int ItemsSelectedAmount { get; private set; }

        public override bool IsValid()
        {
            if (MinObjectsSelected < 0)
            {
                Debug.LogError($"MinObjectSelected is less than zero: {MinObjectsSelected}");
                return false;
            }

            if (MinObjectsSelected > MaxObjectsSelected)
            {
                Debug.LogError($"MinObjectsSelected is greater than MaxObjectsSelected: {MinObjectsSelected} > {MaxObjectsSelected}");
                return false;
            }

            if (MaxObjectsSelected < 0)
            {
                Debug.LogError($"MaxObjectsSelected is less than zero: {MaxObjectsSelected}");
                return false;
            }

            var itemsCount = GetItemsCount();
            if (itemsCount < 1)
            {
                Debug.LogError($"Items count is less than 1: {itemsCount}");
                return false;
            }

            if (MaxObjectsSelected > itemsCount)
            {
                Debug.LogError($"MaxObjectsSelected is greater than items count: {MaxObjectsSelected} > {itemsCount}");
                return false;
            }

            if (MinObjectsSelected > itemsCount)
            {
                Debug.LogError($"MinObjectsSelected is greater than items count: {MinObjectsSelected} > {itemsCount}");
                return false;
            }

            return true;
        }

        public override void DoChoice(params int[] indexes)
        {
            DisableItems();
            ItemsSelectedAmount = indexes.Length;
            foreach (var t in indexes)
                transform.GetChild(t).gameObject.SetActive(true);
            ChunkController.EmitEventGroupChoiceDone(this);
        }

        public override void DoRndChoice(IPseudoRandomNumberGenerator rnd)
        {   
            int[] itemsIndexes = new int[GetItemsCount()];
            for (int i = 0; i < GetItemsCount(); ++i)
                itemsIndexes[i] = i;
            var choices = rnd.FromArray(itemsIndexes, rnd.FromRangeIntInclusive(MinObjectsSelected, MaxObjectsSelected));
            DoChoice(choices);
        }
    }
}
