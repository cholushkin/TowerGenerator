using System.Collections;
using GameLib.Alg;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    // Example:
    // Let's assume there is a group (GroupStack) of 4 items, all disabled: 0000
    // MinIndexSelected = 1
    // Then all possible combinations will be:
    // 0011 - child0 and child1 are always equal to 1 because of MinIndexSelected==1
    // 0111 - add child on top of the stack
    // 1111 - add another child on top of the stack
    public class GroupStack : Group
    {
        public int MinIndexSelected; // Default -1
        public override bool IsValid()
        {
            // Lazy initialization
            if (!IsInitialized())
                Initialize();

            var itemsCount = GetItemsCount();
            if (itemsCount < 1)
            {
                Debug.LogError($"Items count is less than 1: {itemsCount} < 1");
                return false;
            }

            var rightRange = MinIndexSelected >= -1 && MinIndexSelected < itemsCount;
            if (!rightRange)
            {
                Debug.LogError($"{transform.GetDebugName()} MinIndexSelected is : {MinIndexSelected}. Should be -1 to {itemsCount-1}");
                return false;
            }

            return true;
        }

        protected override void SetState(BitArray state, bool notifyChunkController)
        {
            Assert.IsTrue(state.Count == _items.Count);
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
            var rndStackLevel = rnd.FromRangeIntInclusive(0, GetItemsCount() - 1);
            rndStackLevel = Mathf.Max(rndStackLevel, MinIndexSelected);
            EnableItem(rndStackLevel, rnd.YesNo(), notifyChunkController);
        }

        public override void EnableItem(int index, bool flag, bool notifyChunkController)
        {
            Assert.IsTrue(index >= 0);
            Assert.IsTrue(index < GetItemsCount());

            var state = GetState();
            index = Mathf.Max(index, MinIndexSelected);

            for (int i = 0; i < state.Count; ++i)
                state[i] = i <= index;

            state[index] = flag;
            if (index == MinIndexSelected)
                state[index] = true;

            SetState(state, notifyChunkController);
        }
    }
}