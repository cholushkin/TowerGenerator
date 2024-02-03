using System.Collections.Generic;
using UnityEngine;

namespace TowerGenerator
{
    interface IMetaProviderPopulator
    {
        List<MetaBase> FindMetas();
        void Populate(MetaProvider targetMetaProvider);
    }

    public class MetaProviderPopulatorBase : MonoBehaviour, IMetaProviderPopulator
    {
        [SerializeField]
        protected List<MetaBase> Metas;

        public virtual List<MetaBase> FindMetas()
        {
            return Metas;
        }

        public virtual void Populate(MetaProvider targetMetaProvider)
        {
            Metas = FindMetas();
            targetMetaProvider.Populate(Metas);
        }
    }
}