using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class TowerGenerator : MonoBehaviour
    {
        public int SeedTopology;
        public int SeedVisual;
        public int SeedContent;

        public bool GenererateOnStart;

        public TopologyGeneratorsManifoldBase TopologyGeneratorManifold;

        void Reset()
        {
            SeedTopology = -1;
            SeedVisual = -1;
            SeedContent = -1;
            GenererateOnStart = true;
            TopologyGeneratorManifold = GetComponentInChildren<TopologyGeneratorsManifoldBase>();
        }

        void Start()
        {
            Assert.IsNotNull(TopologyGeneratorManifold);

            if (SeedTopology == -1)
            {
                SeedTopology = Random.Range(0, int.MaxValue);
                Debug.Log($"Using random SeedTopology: {SeedTopology}");
            }

            if (SeedVisual == -1)
            {
                SeedVisual = Random.Range(0, int.MaxValue);
                Debug.Log($"Using random SeedVisual: {SeedVisual}");
            }

            if (SeedContent == -1)
            {
                SeedContent = Random.Range(0, int.MaxValue);
                Debug.Log($"Using random SeedContent: {SeedContent}");
            }

            if (GenererateOnStart)
                TopologyGeneratorManifold.StartGenerate((uint)SeedTopology);
        }
    }
}
