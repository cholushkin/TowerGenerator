using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Plugins.Alg;
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

        public override bool IsValid()
        {
            if (MinObjectsSelected < 0)
            {
                Debug.LogError($"{transform.GetDebugName()} MinObjectSelected is less than zero: {MinObjectsSelected}");
                return false;
            }

            if (MinObjectsSelected > MaxObjectsSelected)
            {
                Debug.LogError($"{transform.GetDebugName()} MinObjectsSelected is greater than MaxObjectsSelected: {MinObjectsSelected} > {MaxObjectsSelected}");
                return false;
            }

            if (MaxObjectsSelected < 0)
            {
                Debug.LogError($"{transform.GetDebugName()} MaxObjectsSelected is less than zero: {MaxObjectsSelected}");
                return false;
            }

            var itemsCount = GetItemsCount();
            if (itemsCount < 1)
            {
                Debug.LogError($"{transform.GetDebugName()} Items count is less than 1: {itemsCount}");
                return false;
            }

            if (MaxObjectsSelected > itemsCount)
            {
                Debug.LogError($"{transform.GetDebugName()} MaxObjectsSelected is greater than items count: {MaxObjectsSelected} > {itemsCount}");
                return false;
            }

            if (MinObjectsSelected > itemsCount)
            {
                Debug.LogError($"{transform.GetDebugName()} MinObjectsSelected is greater than items count: {MinObjectsSelected} > {itemsCount}");
                return false;
            }

            return true;
        }

        protected override void SetState(params int[] indexes)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                var needToEnable = indexes.Contains(i);
                ChunkController.SetNodeActiveState(child, needToEnable);
            }
        }

        public override void EnableItem(int index, bool flag)
        {
            var child = transform.GetChild(index);
            ChunkController.SetNodeActiveState(child, flag);
            Assert.IsTrue(GetState().Count >= MinObjectsSelected);
            Assert.IsTrue(GetState().Count <= MaxObjectsSelected);
        }

        public override void SetRandomState(IPseudoRandomNumberGenerator rnd)
        {
            int[] itemsIndexes = new int[GetItemsCount()];
            for (int i = 0; i < GetItemsCount(); ++i)
                itemsIndexes[i] = i;
            var choices = rnd.FromArray(itemsIndexes, rnd.FromRangeIntInclusive(MinObjectsSelected, MaxObjectsSelected));
            SetState(choices);
        }
    }
}
