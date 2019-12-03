using System.Collections;
using UnityEngine;

namespace TowerGenerator
{
    public class TopologyGeneratorsManifoldBase : MonoBehaviour
    {
        public virtual void StartGenerate(uint seed)
        {
            StartCoroutine(GenerateTopology(seed));
        }

        protected virtual IEnumerator GenerateTopology(uint seed)
        {
            return null;
        }

        protected virtual void FinalizeTower(uint seed)
        {

        }

        protected virtual void ResolveDeadlock()
        {

        }
    }
}
