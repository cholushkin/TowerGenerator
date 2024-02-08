using System.Collections.Generic;
using UnityEngine;

namespace TowerGenerator
{
    interface IMetaProviderPopulator<TMeta> where TMeta : MetaBase
    {
        List<TMeta> FindMetas();
        void Populate(MetaProviderGeneric<TMeta> targetMetaProvider);
    }

    public class MetaProviderPopulatorGeneric<TMeta> : MonoBehaviour, IMetaProviderPopulator<TMeta> where TMeta : MetaBase
    {
        [SerializeField]
        protected List<TMeta> Metas;

        public virtual List<TMeta> FindMetas()
        {
            return Metas;
        }

        public virtual void Populate(MetaProviderGeneric<TMeta> targetMetaProvider)
        {
            Metas = FindMetas();
            targetMetaProvider.Populate(Metas);
        }
    }
}