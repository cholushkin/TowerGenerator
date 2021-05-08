using System;
using System.Collections.Generic;
using GameLib;
using GameLib.Random;
using Malee;
using UnityEngine;

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

        //public bool ResetOnProcessorEnter;

        [Reorderable]
        public GeneratorNodesList Nodes;

        [Tooltip("-1 is infinite")] public int NodeCycles;
        public CyclerType NodesCyclerType;

        protected Chooser<GeneratorNodes.NodeItem> _chooser;

        
        void Reset()
        {
            NodeCycles = -1;
            NodesCyclerType = CyclerType.CyclerStraight;

            // by default populate list with all children configs and prototypes
            {
                // todo:
                //var configs = transform.GetComponentsInChildren<GeneratorConfigBase>();
                //Assert.IsTrue(transform.childCount == configs.Length);

                //foreach (var cfg in configs)
                //{
                //    Nodes.Add(new NodeItem {GeneratorNode = cfg.gameObject});
                //}
            }
        }

        public void Init(long seed, Prototype prototype)
        {
            _chooser = new Chooser<NodeItem>(Nodes.ToArray(), NodesCyclerType, seed, NodeCycles);
            var rnd = RandomHelper.CreateRandomNumberGenerator(seed);

            foreach (var node in Nodes)
            {
                var cfg = node.GeneratorNode.GetComponent<GeneratorConfigBase>();
                if (cfg != null)
                    cfg.Init(rnd.ValueInt(), prototype);
            }
        }

        public void OnProcessorEnter()
        {
            _chooser.Reset();
            //if (ResetOnProcessorEnter)
            //{
            //    foreach (var node in Nodes)
            //    {
            //        var cfg = node.GeneratorNode.GetComponent<GeneratorConfigBase>();
            //        if (cfg != null)
            //        {
            //            cfg.ResetSeeds();
            //        }
            //    }
            //}
        }

        public GameObject GetNext()
        {
            var cur = _chooser.GetCurrent();
            _chooser.Step();
            return cur?.GeneratorNode;
        }

        public int GetNodesCount()
        {
            return Nodes.Count;
        }
    }
}