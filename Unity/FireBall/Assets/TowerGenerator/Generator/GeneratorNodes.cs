using System;
using GameLib;
using Malee;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    // list of ConfigGenerators and Prototypes connected with cycler through them
    public class GeneratorNodes : MonoBehaviour
    {
        [Serializable]
        public class NodeItem
        {
            [Tooltip("Prototype or generator config (Cfg|PrototypePrefab|PrototypeNestedWithOverrides)")]
            public GameObject GeneratorNode;
        }

        [Serializable]
        public class GeneratorNodesList : ReorderableArray<NodeItem>
        {
        }

        [Reorderable] public GeneratorNodesList Nodes;

        [Tooltip("-1 is infinite")] public int NodeCycles;
        public CyclerType NodesCyclerType;
        protected Chooser<GeneratorNodes.NodeItem> _chooser;


        // by default populate list with all children configs
        void Reset()
        {
            NodeCycles = -1;
            NodesCyclerType = CyclerType.CyclerStraight;

            var configs = transform.GetComponentsInChildren<GeneratorConfigBase>();
            Assert.IsTrue(transform.childCount == configs.Length);

            foreach (var cfg in configs)
            {
                Nodes.Add(new NodeItem {GeneratorNode = cfg.gameObject});
            }
        }

        public void Init(int seed)
        {
            _chooser = new Chooser<NodeItem>(Nodes.ToArray(), NodesCyclerType, seed, NodeCycles);
        }

        public GameObject GetNext()
        {
            var cur = _chooser.GetCurrent();
            _chooser.Step();
            return cur?.GeneratorNode;
        }
    }
}