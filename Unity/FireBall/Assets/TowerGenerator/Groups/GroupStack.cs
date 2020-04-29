﻿using GameLib.Random;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public class GroupStack : Group
    {
        public int LayerIndexSelected;

        public override bool IsValid()
        {
            throw new System.NotImplementedException();
        }

        public override int GetNumberOfPermutations()
        {
            return GetItemsCount(); 
        }

        public override void DoChoice(params int[] index)
        {
            Assert.IsTrue(index.Length == 1);
            var layer = index[0];
            Assert.IsTrue(layer >= 0);
            Assert.IsTrue(layer < GetItemsCount());

            DisableItems();
            LayerIndexSelected = layer;
            for (int i = 0; i <= layer; ++i)
                transform.GetChild(i).gameObject.SetActive(true);
        }

        public override void DoRndChoice(ref RandomHelper rnd)
        {
            DisableItems();
            LayerIndexSelected = rnd.FromRangeIntInclusive(0, GetItemsCount() - 1);
            for (int i = 0; i <= LayerIndexSelected; ++i)
                transform.GetChild(i).gameObject.SetActive(true);
        }

        public override void DoRndMinimalChoice(ref RandomHelper rnd)
        {
            DoChoice(0);
        }
    }
}