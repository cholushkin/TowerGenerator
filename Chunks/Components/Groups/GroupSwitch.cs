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

        protected override void SetState(params int[] index)
        {
            Assert.IsTrue(index.Length == 1);
            var itemIndex = index[0];
            Assert.IsTrue(itemIndex >= 0);
            Assert.IsTrue(itemIndex < GetItemsCount());

            for (int i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                var needToEnable = i == itemIndex;
                ChunkController.SetNodeActiveState(child, needToEnable);
            }
        }

        public override void EnableItem(int index, bool flag)
        {
            if(flag == false)
                Debug.LogError($"GroupSwitch is not supposed to disable item {gameObject.transform.GetDebugName()}");

            SetState(index);
        }

        public override void SetRandomState(IPseudoRandomNumberGenerator rnd)
        {
            Assert.IsTrue(GetItemsCount() > 0);
            SetState(rnd.FromRangeIntInclusive(0, GetItemsCount() - 1));
        }
    }
}