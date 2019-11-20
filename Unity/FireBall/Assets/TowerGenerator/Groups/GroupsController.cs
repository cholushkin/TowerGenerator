﻿using System;
using Assets.Plugins.Alg;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;


namespace TowerGenerator
{
    public class GroupsController : MonoBehaviour
    {
        public class ProcessingNode
        {
            public Transform Transform;
            public Group Group;
        }

        private TreeNode<ProcessingNode> _tree;
        public Bounds BBMax;
        public Bounds BBMin;
        public GroupStack GlobalStackLayers;
        public long Seed = -1;

        void Awake()
        {
            if (Seed == -1)
                Seed = Random.Range(0, Int32.MaxValue);
        }

        public void BuildImpactTree()
        {
            ForEachChildrenRecursive(transform, null);
        }

        public Bounds CalculateBBMax()
        {
            foreach (var treeNode in _tree.TraverseDepthFirstPostOrder())
            {
                if (treeNode.Data.Transform.GetComponent<Connectors>() != null)
                    treeNode.Data.Transform.gameObject.SetActive(false);
                else
                    treeNode.Data.Transform.gameObject.SetActive(true);
            }

            BBMax = _tree.Data.Transform.gameObject.BoundBox();
            BBMax.center = Vector3.zero;
            Debug.Log($"BBmax:{BBMax}");
            return BBMax;
        }

        [ContextMenu("CalculateBBSize")]
        public Vector3 CalculateBBSize()
        {
            if (_tree == null)
                BuildImpactTree();
            BBMax = _tree.Data.Transform.gameObject.BoundBox();
            BBMax.center = Vector3.zero;
            Debug.Log($"BBmax:{BBMax}");
            return BBMax.size;
        }


        public bool SetMaximizedFitRndConfiguration(Vector3 maxBBSize)
        {
            var origSeed = Seed;

            const int iterations = 32;
            float maxVolume = 0;
            long fitSeed = -1;
            for (int i = 0; i < iterations; i++)
            {
                var s = Seed;
                SetRndConfiguration();
                var bbxize = CalculateBBSize();
                var volume = bbxize.x * bbxize.y * bbxize.z;
                Debug.Log($"Volume {volume}");

                var isInside = bbxize.x <= maxBBSize.x && bbxize.y <= maxBBSize.y && bbxize.z <= maxBBSize.z;

                if (volume > maxVolume && isInside)
                {
                    maxVolume = volume;
                    fitSeed = s;
                }
            }

            if (fitSeed == -1)
            {
                Debug.Log($"Can't fit! Using SetMinimizedRndConfiguration with original seed {origSeed}");
                Seed = origSeed;
                SetMinimizedRndConfiguration();
                var bbxize = CalculateBBSize();
                var isInside = bbxize.x <= maxBBSize.x && bbxize.y <= maxBBSize.y && bbxize.z <= maxBBSize.z;
                Debug.LogError(
                    $"Can't fit even with minimal cfg resulting bb:{bbxize} in requested bb:{maxBBSize} - obj:{gameObject}");
                return false;
            }

            Debug.Log($"Max Volume {maxVolume} seed for it {fitSeed}");
            Seed = fitSeed;
            SetRndConfiguration();
            return true;
        }

        [ContextMenu("SetMinimizedRndConfiguration")]
        public void SetMinimizedRndConfiguration()
        {
            SetRndConfiguration(true);
        }

        [ContextMenu("SetRndConfiguration")]
        public void SetRndConfiguration()
        {
            SetRndConfiguration(false);
        }

        public void SetRndConfiguration(bool minBB = false)
        {
            RandomHelper rnd = new RandomHelper(Seed);
            Debug.Log(rnd.GetCurrentSeed());

            // enable all parts
            transform.ForEachChildrenRecursive(t => t.gameObject.SetActive(true));

            if (_tree == null)
                BuildImpactTree();

            // setup global layer
            var currentGlobalLayer = -1;
            if (GlobalStackLayers != null)
            {
                if (minBB)
                    GlobalStackLayers.DoRndMinimalChoice(ref rnd);
                else
                    GlobalStackLayers.DoRndChoice(ref rnd);
                currentGlobalLayer = GlobalStackLayers.LayerIndexSelected;
            }

            foreach (var treeNode in _tree.TraverseDepthFirstPostOrder())
            {
                var group = treeNode.Data.Group;
                if (group != null)
                {
                    if (group == GlobalStackLayers)
                        continue;

                    if (!treeNode.Data.Transform.gameObject.activeInHierarchy)
                        continue;

                    var groupIsActual = currentGlobalLayer <= group.PropagatedTo;

                    if (groupIsActual)
                    {
                        if (minBB)
                            group.DoRndMinimalChoice(ref rnd);
                        else
                            group.DoRndChoice(ref rnd);
                    }
                    else
                        group.DisableItems();
                }
            }

            Seed = rnd.GetCurrentSeed();
        }


        public void CalculateBBMin()
        {
            foreach (var treeNode in _tree.TraverseBreadthFirst())
            {
            }
        }

        public void ForEachChildrenRecursive(Transform iTrans, TreeNode<ProcessingNode> parent)
        {
            // visit
            var group = iTrans.GetComponent<Group>();
            var node = new TreeNode<ProcessingNode>(
                new ProcessingNode
                {
                    Transform = iTrans,
                    Group = group
                });

            var groupStack = group as GroupStack;
            if (groupStack)
            {
                if (groupStack.PropagatedTo == -1)
                {
                    GlobalStackLayers = groupStack;
                }
            }

            parent?.AddChild(node);

            if (_tree == null)
            {
                Assert.IsNull(parent);
                _tree = node;
            }

            for (int i = 0; iTrans.childCount != i; ++i)
            {
                ForEachChildrenRecursive(iTrans.GetChild(i), node);
            }
        }

        public Vector2 GetAmmountOfGroups()
        {
            return new Vector2(transform.GetComponentsInChildren<Group>().Length,
                transform.GetComponentsInChildren<Group>(true).Length);
        }
    }
}