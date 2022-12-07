using System.Collections;
using Assets.Plugins.Alg;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    // GroupSet controls some subset of its children
    // Example:
    // Let's assume there is a group (GroupSet) of 4 items, all disabled: 0000
    // MinObjectsSelected = 1, MaxObjectsSelected = 3
    // Then all possible combinations will be:
    // 1000 1100 0101 1101
    // 0100 0110 1110
    // 0010 0011 0111
    // 0001 1010 1011

    public class GroupSet : Group
    {
        public int MinObjectsSelected; // Default == 0
        public int MaxObjectsSelected; // Default == transform.childCount

        public override bool IsValid()
        {
            // Lazy initialization
            if (!IsInitialized())
                Initialize();

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

        protected override void SetState(BitArray state, bool notifyChunkController)
        {
            Assert.IsTrue(state.Count == _items.Count);
            Assert.IsTrue(GetEnabledItemsCount(state) >= MinObjectsSelected);
            Assert.IsTrue(GetEnabledItemsCount(state) <= MaxObjectsSelected);

            for (int i = 0; i < state.Count; ++i)
            {
                if (notifyChunkController)
                    ChunkController.SetNodeActiveState(_items[i], state[i]);
                else
                    _items[i].gameObject.SetActive(state[i]);
            }
        }

        public override void SetRandomState(IPseudoRandomNumberGenerator rnd, bool notifyChunkController)
        {
            // Prepare the array of index
            int[] itemsIndexes = new int[GetItemsCount()];
            for (int i = 0; i < GetItemsCount(); ++i)
                itemsIndexes[i] = i;

            // Take randomly from MinObjectsSelected to MaxObjectsSelected elements from index array
            var choices = rnd.FromArray(itemsIndexes, rnd.FromRangeIntInclusive(MinObjectsSelected, MaxObjectsSelected));

            // Set bits 
            BitArray newState = new BitArray(GetItemsCount(), false);
            foreach (var choiceIndex in choices)
                newState[choiceIndex] = true;

            SetState(newState, notifyChunkController);
        }

        public override void EnableItem(int index, bool flag, bool notifyChunkController)
        {
            Assert.IsTrue(index < GetItemsCount());
            Assert.IsTrue(index >= 0);
            Assert.IsTrue(GetEnabledItemsCount(_state) >= MinObjectsSelected);
            Assert.IsTrue(GetEnabledItemsCount(_state) <= MaxObjectsSelected);
            var state = GetState();
            state[index] = flag;

            // Auto adjust to the range [MinObjectsSelected, MaxObjectsSelected] of enabled items in case of wrong range
            var enabledNumber = GetEnabledItemsCount(state);
            if (enabledNumber < MinObjectsSelected || enabledNumber > MaxObjectsSelected)
            {
                for (int i = 0; i < state.Count; ++i)
                {
                    if( i == index)
                        continue;
                    if (flag) // user requested to set bit and exceeded amount of max allowed items 
                    {
                        if (state[i])
                        {
                            state[i] = false;
                            break;
                        }
                    }
                    else // user requested to unset bit and got less than a minimum amount of allowed items
                    {
                        if (state[i] == false)
                        {
                            state[i] = true;
                            break;
                        }
                    }
                }
            }

            Assert.IsTrue(GetEnabledItemsCount(state) >= MinObjectsSelected);
            Assert.IsTrue(GetEnabledItemsCount(state) <= MaxObjectsSelected);

            SetState(state, notifyChunkController);
        }
    }
}
