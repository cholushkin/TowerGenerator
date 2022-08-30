using Assets.Plugins.Alg;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public class GroupStack : Group
    {
        public int MinIndexSelected; // default -1
        public override bool IsValid()
        {
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

        protected override void SetState(params int[] index)
        {
            Assert.IsTrue(index.Length == 1);
            var stackTo = index[0];
            Assert.IsTrue(stackTo  >= 0);
            Assert.IsTrue(stackTo < GetItemsCount());

            for (int i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                var needToEnable = i <= stackTo;
                ChunkController.SetNodeActiveState(child, needToEnable);
            }
        }

        public override void EnableItem(int index, bool flag)
        {
            if (flag)
                SetState(index);
            else
                SetState(Mathf.Min(index - 1, 0));
        }

        public override void SetRandomState(IPseudoRandomNumberGenerator rnd)
        {
            SetState(rnd.FromRangeIntInclusive(0, GetItemsCount() - 1));
        }
    }
}