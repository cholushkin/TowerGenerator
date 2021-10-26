using System.Collections.Generic;
using GameLib.Random;
using UnityEngine;


namespace TowerGenerator
{
    public abstract class Group : BaseComponent
    {
        // Sets a group to one of possible states.
        // The state controlled by indexes. Depending on a group type it could be one index or set of indexes.
        protected abstract void SetState(params int[] index);

        // return indexes of activated children
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

        public virtual void SetInitialState()
        {
            for (int i = 0; i < transform.childCount; ++i)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        public abstract void SetRandomState(IPseudoRandomNumberGenerator rnd);


        // enable item in unique to group type way
        public abstract void EnableItem(int index, bool flag); 

        // Items number under control of this group
        public virtual int GetItemsCount()
        {
            return transform.childCount;
        }
    }
}