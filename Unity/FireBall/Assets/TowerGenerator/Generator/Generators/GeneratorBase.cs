using System.Collections;
using System.Collections.Generic;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    public class GeneratorBase : MonoBehaviour
    {
        public class GeneratorState
        {
            public GeneratorState(TreeNode<Blueprint.Segment> entryNode)
            {
                NodeEntry = entryNode;
            }

            public List<TreeNode<Blueprint.Segment>> SegmentsActive = new List<TreeNode<Blueprint.Segment>>(16);
            public List<TreeNode<Blueprint.Segment>> SegmentsOpened = new List<TreeNode<Blueprint.Segment>>(16);
            public List<TreeNode<Blueprint.Segment>> SegmentsCreated = new List<TreeNode<Blueprint.Segment>>(16);

            public TreeNode<Blueprint.Segment> NodeDeadlock;
            public TreeNode<Blueprint.Segment> NodeEntry;

            public int Iteration { get; internal set; }
        }

        public GeneratorState State { get; protected set; }
        public GeneratorConfigBase Config { get; private set; }
        protected RandomHelper _rnd;

        public GeneratorBase(long seed, GeneratorConfigBase cfg)
        {
        }
    }


}

