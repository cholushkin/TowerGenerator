﻿using System.Collections.Generic;
using GameLib.Random;

namespace TowerGenerator
{
    public class GroupUser : Group
    {
        public override bool IsValid()
        {
            throw new System.NotImplementedException();
        }

        public override int GetNumberOfPermutations()
        {
            return 0;
        }

        public override void DoChoice(params int[] index)
        {
            throw new System.NotImplementedException();
        }

        public override void DoRndChoice(ref RandomHelper rnd)
        {
            DisableItems();
        }

        public override void DoRndMinimalChoice(ref RandomHelper rnd)
        {
            DisableItems();
        }
    }
}