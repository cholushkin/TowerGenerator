using GameLib.Random;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public class GroupSwitch : Group
    {
        public int ItemSelected { get; private set; }

        public override bool IsValid()
        {
            var childCount = GetItemsCount();
            if (childCount < 2)
                return false;
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
        }

        public override void DoRndChoice(ref RandomHelper rnd)
        {
            Assert.IsTrue(GetItemsCount() > 0);
            DoChoice(rnd.FromRangeIntInclusive(0, GetItemsCount() - 1));
        }
    }
}