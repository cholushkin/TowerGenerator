using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    // Randomly enables some options from the group
    public class GroupRndSet : Group
    {
        public int MinObjectsSelected; // default 0
        public int MaxObjectsSelected; // default transform.childCount

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
            MinObjectsSelected = 0;
            MaxObjectsSelected = transform.childCount;
        }

        public override bool SetProp(string propName, object value) // return false if cannot find property 
        {
            var baseResult = base.SetProp(propName, value);
            if (propName == "MaxObjectsSelected")
            {
                var itemCount = transform.childCount;
                if (value is string strVal)
                {
                    if (strVal.ToLower() == "all")
                        MaxObjectsSelected = itemCount;
                }
                else
                {
                    var valInt = (int)value;
                    MaxObjectsSelected = Mathf.Clamp(valInt, 0, itemCount);
                    Assert.IsTrue(valInt == MaxObjectsSelected);
                }
                return true;
            }

            if (propName == "MinObjectsSelected")
            {
                var itemCount = transform.childCount;
                {
                    var valInt = (int)value;
                    MinObjectsSelected = Mathf.Clamp(valInt, 0, itemCount);
                    Assert.IsTrue(valInt == MinObjectsSelected, $"user MinObjectsSelected parsed:{valInt}, clamped {MinObjectsSelected}");
                }
                return true;
            }
            return baseResult;
        }
    }
}
