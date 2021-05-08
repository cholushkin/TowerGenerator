using UnityEngine;

namespace TowerGenerator
{
    public class MetaProviderManager : MonoBehaviour
    {
        public MetaProvider[] MetaProviders { get; private set; }

        public void Init()
        {
            MetaProviders = GetComponentsInChildren<MetaProvider>();
            foreach (var metaProvider in MetaProviders)
            {
                metaProvider.Init();
            }
        }
    }
}