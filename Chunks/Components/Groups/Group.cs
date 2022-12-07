using System.Collections;
using System.Collections.Generic;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public abstract class Group : BaseComponent
    {
        protected List<Transform> _items;
        protected BitArray _state;

        public override void Initialize()
        {
            _items = new List<Transform>(transform.childCount);
            for (int i = 0; transform.childCount != i; ++i)
                if (transform.GetChild(i).GetComponent<IgnoreGroupItem>() == null)
                {
                    _items.Add(transform.GetChild(i));
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            _state = new BitArray(_items.Count, false);
        }

        public bool IsInitialized()
        {
            return _state != null;
        }

        public List<Transform> GetChildren() => _items;

        // Sets a group to one of possible states.
        // The state controlled by indexes. Depending on a group type it could be one index or set of indexes.
        protected abstract void SetState(BitArray state, bool notifyChunkController);

        // Returns state of each children
        public BitArray GetState()
        {
            return _state;
        }

        // Set group random state in specific for a group way
        public abstract void SetRandomState(IPseudoRandomNumberGenerator rnd, bool notifyChunkController);


        // Enable item in unique to group type way
        public abstract void EnableItem(int index, bool flag, bool notifyChunkController); 

        // Get items number under control of this group
        public virtual int GetItemsCount()
        {
            return _items.Count;
        }

        // Get enabled items count
        public int GetEnabledItemsCount(BitArray state)
        {
            Assert.IsTrue(state.Count == _state.Count);
            int count = 0;
            for (int i = 0; i < state.Length; i++)
                if (state[i])
                    count++;
            return count;
        }
    }
}