using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public class GroupStack : Group
    {
        public override bool IsValid()
        {
            var itemsCount = GetItemsCount();
            if (itemsCount < 2)
            {
                Debug.LogError($"Items count is less than 2: {itemsCount} < 2");
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