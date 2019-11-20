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

        public override void DoRndChoice(ref RandomHelper rnd)
        {
            Assert.IsTrue(GetItemsCount() > 0);
            DisableItems();
            ItemIndexSelected = rnd.FromRangeIntInclusive(0, GetItemsCount() - 1);
            transform.GetChild(ItemIndexSelected).gameObject.SetActive(true);
        }

        public override void DoRndMinimalChoice(ref RandomHelper rnd)
        {
            Assert.IsTrue(GetItemsCount() > 0);
            DisableItems();
            ItemIndexSelected = 0;
            transform.GetChild(ItemIndexSelected).gameObject.SetActive(true);
        }
    }
}