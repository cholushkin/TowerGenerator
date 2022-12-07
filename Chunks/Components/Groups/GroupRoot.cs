using System;
using System.Collections;
using Assets.Plugins.Alg;
using GameLib.Random;


namespace TowerGenerator
{
    // GroupRoot is used as a proxy root group for impact tree calculation
    public class GroupRoot  : Group 
    {
        public override void Initialize()
        {
        }

        public override bool IsValid()
        {
            return true;
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