using GameLib.Random;
using UnityEngine;


namespace TowerGenerator
{
    public abstract class Group : BaseComponent
    {
        // Sets a group to one of possible states.
        // The state controlled by indexes. Depending on a group it could be one index or set of indexes.
        public abstract void DoChoice(params int[] index);

        // Do DoChoice call with a random choice
        public abstract void DoRndChoice(IPseudoRandomNumberGenerator rnd);

        public void DisableItems()
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(false);
        }

        public virtual int GetItemsCount()
        {
            return transform.childCount;
        }
    }
}