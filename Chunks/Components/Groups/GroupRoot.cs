using GameLib.Random;


namespace TowerGenerator
{
    // used as a proxy root group for impact tree calculation
    public class GroupRoot  : Group 
    {
        public override bool IsValid()
        {
            return true;
        }

        public override void DoChoice(params int[] index)
        {
        }

        public override void DoRndChoice(IPseudoRandomNumberGenerator rnd)
        {
        }
    }
}