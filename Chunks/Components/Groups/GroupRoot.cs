using System;
using System.Collections;
using Assets.Plugins.Alg;
using GameLib.Random;


namespace TowerGenerator
{
    // GroupRoot is used as a proxy root group for impact tree calculation
    public class GroupRoot  : Group 
    {

        public override bool IsValid()
        {
            return true;
        }

        public override void DisableAllItems()
        {
            // override disable all items and do nothing. Root group can't disable or enable items
        }

        protected override void SetState(BitArray state, bool notifyChunkController)
        {
        }

        public override void SetRandomState(IPseudoRandomNumberGenerator rnd, bool notifyChunkController)
        {
        }

        public override void EnableItem(int index, bool flag, bool notifyChunkController)
        {
        }
    }
}