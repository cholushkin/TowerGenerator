using System.Collections.Generic;
using GameLib.Random;

namespace TowerGenerator
{
    public class GroupUser : Group
    {
        public override int GetNumberOfPermutations()
        {
            return 0;
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