using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public class GroupStack : Group
    {
        public int ItemStacked { get; private set; }

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

        public override void DoChoice(params int[] index)
        {
            Assert.IsTrue(index.Length == 1);
            ItemStacked = index[0];
            Assert.IsTrue(ItemStacked >= 0);
            Assert.IsTrue(ItemStacked < GetItemsCount());

            DisableItems();

            for (int i = 0; i <= ItemStacked; ++i)
                transform.GetChild(i).gameObject.SetActive(true);

            ChunkController.EmitEventGroupChoiceDone(this);
        }

        public override void DoRndChoice(IPseudoRandomNumberGenerator rnd)
        {
            DoChoice(rnd.FromRangeIntInclusive(0, GetItemsCount() - 1));
        }
    }
}