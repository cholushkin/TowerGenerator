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

        protected override void SetState(params int[] index)
        {
        }

        public override void SetRandomState(IPseudoRandomNumberGenerator rnd)
        {
        }

        public override void EnableItem(int index, bool flag)
        {
        }
    }
}