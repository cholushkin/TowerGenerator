using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public abstract class BasePropImporter : MonoBehaviour
    {
        protected int GetMyLayer()
        {
            var layer = GetComponentInParent<Layer>();
            if (layer != null)
            {
                Assert.IsNotNull(layer.transform.parent.GetComponent<GroupStack>());
                for (int i = 0; i < layer.transform.parent.childCount; ++i)
                    if (layer.transform.parent.GetChild(i).GetComponent<Layer>() == layer)
                        return i;
            }

            return -1;
        }

        protected int GetLayersCount()
        {
            var ent = GetComponentInParent<Entity>();
            if (ent == null)
                return -1;
            var groupStack = ent.transform.GetChild(0).GetComponent<GroupStack>();
            Assert.IsNotNull(groupStack);
            return groupStack.GetItemsCount();
        }

        public abstract void SetDefaultValues();

        public virtual bool SetProp(string propName, object value) // return false if cannot find property 
        {
            return false;
        }
    }
}