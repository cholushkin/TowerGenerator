using System.Collections.Generic;
using GameLib.Random;

namespace TowerGenerator
{
    public class GroupUser : Group
    {
        public override bool IsValid()
        {
            var childCount = GetItemsCount();
            if (childCount < 1)
                return false;
            return true;
        }

        public override void DoChoice(params int[] index)
        {
            throw new System.NotImplementedException();
        }

        public override void DoRndChoice(ref RandomHelper rnd)
        {
            throw new System.NotImplementedException();
        }
    }
}