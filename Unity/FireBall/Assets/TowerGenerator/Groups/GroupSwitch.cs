using System.Collections;
using System.Collections.Generic;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public class GroupSwitch : Group
    {
        public int ItemIndexSelected;

        public override int GetNumberOfPermutations()
        {
            return GetItemsCount();
        }

        public override void DoChoice(params int[] index)
        {
            Assert.IsTrue(index.Length == 1);
            ItemIndexSelected = index[0];
            transform.GetChild(ItemIndexSelected).gameObject.SetActive(true);
        }

        public override void DoRndChoice(ref RandomHelper rnd)
        {
            Assert.IsTrue(GetItemsCount() > 0);
            DisableItems();
            DoChoice(rnd.FromRangeIntInclusive(0, GetItemsCount() - 1));
        }

        public override void DoRndMinimalChoice(ref RandomHelper rnd)
        {
            Assert.IsTrue(GetItemsCount() > 0);
            DisableItems();
            DoChoice(0);
        }
    }
}