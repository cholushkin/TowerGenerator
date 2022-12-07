using System.Collections;
using Assets.Plugins.Alg;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public class GroupSwitch : Group
    {
        public override bool IsValid()
        {
            var itemsCount = GetItemsCount();
            if (itemsCount < 2)
            {
                Debug.LogError($"Items count is less than 2: {itemsCount}. On '{transform.GetDebugName()}'");
                return false;
            }
            return true;
        }

        protected override void SetState(BitArray state, bool notifyChunkController)
        {
            Assert.IsTrue(state.Count == _items.Count);
            Assert.IsTrue(GetEnabledItemsCount(state) == 1);

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
            var rndIndex = rnd.FromRangeIntInclusive(0, GetItemsCount() - 1);
            EnableItem(rndIndex, true, notifyChunkController);
        }

        public override void EnableItem(int index, bool flag, bool notifyChunkController)
        {
            Assert.IsTrue(index < GetItemsCount());
            Assert.IsTrue(index >= 0);
            Assert.IsTrue(GetEnabledItemsCount(_state) == 1);

            var state = GetState();
            state[index] = flag;

            // Auto enable in case of zero number of enabled items
            var enabledNumber = GetEnabledItemsCount(state);
            if (enabledNumber == 0)
            {
                for (int i = 0; i < state.Count; ++i)
                {
                    if (i == index)
                        continue;
                    state[i] = true;
                    break;
                }
            }

            Assert.IsTrue(GetEnabledItemsCount(state) == 1);
            SetState(state, notifyChunkController);
        }
    }
}