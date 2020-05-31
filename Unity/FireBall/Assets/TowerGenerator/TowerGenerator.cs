using System.Linq;
using Assets.Plugins.Alg;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    [RequireComponent(typeof(GeneratorProcessor))]
    public class TowerGenerator : MonoBehaviour
    {
        public Prototype InitialPrototype;
        public GeneratorProcessor Processor;
        public bool DoGenerateOnStart;

        [Range(3, 16)]
        public int PrototypesNestingDepthLevel;
        public Transform OutcomeRoot;

        private int _stackLevel;


        void Start()
        {
            Assert.IsNotNull(InitialPrototype);
            Assert.IsNotNull(Processor);
            Assert.IsNotNull(OutcomeRoot);

            if (DoGenerateOnStart)
                Generate();
        }

        public void Generate()
        {
            // create transform for Prototypes
            var prototypeTransform = new GameObject("Prototype").transform;
            prototypeTransform.SetParent(transform);

            _stackLevel = 0;
            InstantiatePrototypes(InitialPrototype.gameObject, prototypeTransform, 0);
            PropagateSeeds(prototypeTransform);

            Processor.StartGenerate(InitialPrototype, OutcomeRoot);
        }

        private void PropagateSeeds( Transform prototypes )
        {
            int seedTopologyRnd = Random.Range(0, int.MaxValue);
            int seedContentRnd = Random.Range(0, int.MaxValue);
            int seedVisaulRnd = Random.Range(0, int.MaxValue);


            var protoBehs = prototypes.GetComponentsInChildren<Prototype>();
            foreach (var prototype in protoBehs)
            {
                if (prototype.SeedTopology == -1)
                    prototype.SeedTopology = seedTopologyRnd;
                if (prototype.SeedContent == -1)
                    prototype.SeedContent = seedContentRnd;
                if (prototype.SeedVisual == -1)
                    prototype.SeedVisual = seedVisaulRnd;
            }
        }

        private Transform InstantiatePrototypes(GameObject prototypeOrConfigPrefab, Transform root, int level)
        {
            if (level >= PrototypesNestingDepthLevel)
            {
                Debug.LogError($"Reached nesting level {level}");
                return null;
            }
            var gObj = Instantiate(prototypeOrConfigPrefab, root.transform);
            gObj.name = prototypeOrConfigPrefab.name;

            var prototype = gObj.GetComponent<Prototype>();
            if (prototype == null)
                return gObj.transform;

            // instantiate prefabs (non-overriden cfgs or protos)
            var nodesCount = prototype.GeneratorNodes.GetNodesCount();
            var created = prototype.GeneratorNodes.transform.Children().ToArray();

            for (int i = 0; i < nodesCount; ++i)
            {
                var node = prototype.GeneratorNodes.Nodes[i].GeneratorNode;
                if (created.All(x => x != node.transform))
                {
                    var createdFromPrefab = InstantiatePrototypes(node, prototype.GeneratorNodes.transform, level + 1);
                    if (createdFromPrefab)
                        prototype.GeneratorNodes.Nodes[i].GeneratorNode = createdFromPrefab.gameObject;
                }
            }
            return gObj.transform;
        }
    }
}
