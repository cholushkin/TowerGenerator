using System.Collections.Generic;
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

        public List<int> GetState()
        {
            List<int> actives = new List<int>(transform.childCount);
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeInHierarchy)
                    actives.Add(child.GetSiblingIndex());
            }
            return actives;
        }

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