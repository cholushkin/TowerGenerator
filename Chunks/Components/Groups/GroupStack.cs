using System.Collections;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;
using Random = GameLib.Random.Random;


namespace TowerGenerator
{
    // Example:
    // Let's assume there is a group (GroupStack) of 3 items, all disabled: 000
    // Possible states: 000 100 110 111
    public class GroupStack : Group
    {
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

        public override void SetRandomState(Random rnd, bool notifyChunkController)
        {
            var rndStackLevel = rnd.RangeInclusive(0, GetItemsCount() - 1);
            EnableItem(rndStackLevel, rnd.YesNo(), notifyChunkController);
        }

        public override void EnableItem(int index, bool flag, bool notifyChunkController)
        {
            Assert.IsTrue(index >= 0);
            Assert.IsTrue(index < GetItemsCount());

            var state = GetState();

            for (int i = 0; i < state.Count; ++i)
                state[i] = i <= index;

            state[index] = flag;

            SetState(state, notifyChunkController);
        }
    }
}