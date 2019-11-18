using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class TowerGenerator : MonoBehaviour
    {
        public int SeedTopology;
        public int SeedVisual;
        public int SeedContent;

        public bool GenererateOnAwake;

        public TopologyGeneratorsManifoldBase TopologyGeneratorManifold;

        void Awake()
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

            if (GenererateOnAwake)
                TopologyGeneratorManifold.StartGenerate((uint)SeedTopology);
        }
    }
}
