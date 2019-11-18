using System;
using Assets.Plugins.Alg;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public abstract class Group : BasePropImporter
    {
        public int MaxLayerIndexPropagated; // by default it is propagated only to current layer (-1 ignoring)
        protected string MaxLayerIndexPropagatedAll = "all";

        public virtual int GetItemsCount()
        {
            return transform.childCount;
        }

        public override void SetDefaultValues()
        {
            MaxLayerIndexPropagated = GetMyLayer();
        }

        public override bool SetProp(string propName, object value) // return false if cannot find property 
        {
            var baseResult = base.SetProp(propName, value);
            if (propName == "MaxLayerIndexPropagated")
            {
                var itemCount = GetLayersCount();
                if (value is string strVal)
                {
                    if (strVal.ToLower() == MaxLayerIndexPropagatedAll)
                        MaxLayerIndexPropagated = itemCount - 1;
                }
                else
                {
                    var valInt = (int) value;
                    MaxLayerIndexPropagated = Mathf.Clamp(valInt, GetMyLayer(), itemCount - 1);
                    Assert.IsTrue(valInt == MaxLayerIndexPropagated,
                        $"Obj {this.transform.GetDebugName()} Value parsed {valInt}, value clamped {MaxLayerIndexPropagated}");
                }

                return true;
            }

            return baseResult;
        }

        public virtual int GetAmountOfTransformImpact()
        {
            return transform.childCount;
        }

        public abstract void DoRndChoice(ref RandomHelper rnd);
        public abstract void DoRndMinimalChoice(ref RandomHelper rnd);

        public void DisableItems()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}