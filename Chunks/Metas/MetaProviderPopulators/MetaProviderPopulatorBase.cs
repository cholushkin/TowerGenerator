using System.Collections.Generic;
using UnityEngine;

namespace TowerGenerator
{
    interface IMetaProviderPopulator
    {
        void FindMetas();
    }

    [RequireComponent(typeof(MetaProvider))]
    public abstract class MetaProviderPopulatorBase : MonoBehaviour, IMetaProviderPopulator
    {
        public MetaProvider Target;
        public bool PopulateOnAwake;
        public List<MetaBase> FoundMetas;

        void Reset()
        {
            Target = GetComponent<MetaProvider>();
        }

        void Awake()
        {
            if (PopulateOnAwake)
                Populate();
        }

        public abstract void FindMetas();    

        [ContextMenu("Populate")]
        void Populate()
        {
            FindMetas();
            Target.Populate(FoundMetas);
        }
    }
}
