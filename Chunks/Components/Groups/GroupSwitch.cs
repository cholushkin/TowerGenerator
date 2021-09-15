using Assets.Plugins.Alg;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public class GroupSwitch : Group
    {
        public int ItemSelected { get; private set; }

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

        public override void DoChoice(params int[] index)
        {
            Assert.IsTrue(index.Length == 1);
            ItemSelected = index[0];
            Assert.IsTrue(ItemSelected >= 0);
            Assert.IsTrue(ItemSelected < GetItemsCount());

            DisableItems();
            transform.GetChild(ItemSelected).gameObject.SetActive(true);
            ChunkController.EmitEventGroupChoiceDone(this);
        }

        public override void DoRndChoice(IPseudoRandomNumberGenerator rnd)
        {
            Assert.IsTrue(GetItemsCount() > 0);
            DoChoice(rnd.FromRangeIntInclusive(0, GetItemsCount() - 1));
        }
    }
}