using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    [RequireComponent(typeof(GeneratorProcessor))]
    public class TowerGenerator : MonoBehaviour
    {
        public Prototype InitialPrototypePrefab;
        public GeneratorProcessor Processor;
        public bool DoGenerateOnStart;

        [Range(3, 16)]
        public int PrototypesNestingDepthLevel;
        public Transform OutcomeRoot;
        [Tooltip("Seed used for initialize configs with -1 seeds")]
        public long ControlSeed;
        public bool IsHomogeneousPrototypes;

        private Prototype _initialPrototype;

        void Awake()
        {
            Init();
        }   

        void Start()
        {
            if (DoGenerateOnStart)
                Generate();
        }

        void Init()
        {
            Assert.IsNotNull(InitialPrototypePrefab);
            Assert.IsNotNull(Processor);
            Assert.IsNotNull(OutcomeRoot);

            // create transform for Prototypes
            var prototypeTransform = new GameObject("Prototype").transform;
            prototypeTransform.SetParent(transform);

            var proto = InstantiatePrototypes(InitialPrototypePrefab.gameObject, prototypeTransform, 0);
            _initialPrototype = proto.GetComponent<Prototype>();
            InitPrototypes(prototypeTransform);
        }

        public void Generate()
        {
            Processor.StartGenerate(_initialPrototype, OutcomeRoot);
        }

        private void InitPrototypes( Transform prototypes )
        {
            var rnd = RandomHelper.CreateRandomNumberGenerator(ControlSeed);
            if (ControlSeed == -1)
                ControlSeed = rnd.GetState().AsNumber();

            var protoBehs = prototypes.GetComponentsInChildren<Prototype>();
            foreach (var prototype in protoBehs)
            {
                prototype.Init(rnd.GetState().AsNumber());
                if (!IsHomogeneousPrototypes)
                    rnd.Next();
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

            for (int i = 0; i < nodesCount; ++i)
            {
                var node = prototype.GeneratorNodes.Nodes[i].GeneratorNode;
                if (node.transform.parent != prototype.GeneratorNodes.transform)
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
