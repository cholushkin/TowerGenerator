using UnityEngine;

namespace TowerGenerator
{
    public class MetaProviderInitOnAwake : MonoBehaviour
    {
        void Awake()
        {
            GetComponent<MetaProvider>().Init();
        }
    }
}
