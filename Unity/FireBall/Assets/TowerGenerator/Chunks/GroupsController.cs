using System;
using System.Collections.Generic;
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
        public long Seed = -1;
        public GroupStack DimensionStack { get; private set; }
        public Connectors Connectors { get; private set; }

        private TreeNode<Group> _tree;
        private Dictionary<string, List<Transform>> _suppression;
        private Dictionary<string, List<Transform>> _induction;


        public void Init() // configure
        {
            if (Seed == -1)
                Seed = Random.Range(0, Int32.MaxValue);
            DimensionStack = GetComponentInChildren<GroupStack>();
            Assert.IsNotNull(DimensionStack, $"{gameObject.transform.GetDebugName()}");
            BuildImpactTree();
#if DEBUG
            Validate();
#endif
        }

        public Bounds CalculateBB()
        {
            Assert.IsNotNull(_tree);
            return _tree.Data.gameObject.BoundBox();
        }

        public void SetConfiguration()
        {
            RandomHelper rnd = new RandomHelper(Seed);
            Debug.Log(rnd.GetCurrentSeed());

            // enable all parts
            transform.ForEachChildrenRecursive(t => t.gameObject.SetActive(true));

            foreach (var treeNode in _tree.TraverseDepthFirstPostOrder())
            {
                var group = treeNode.Data;
                if (group != null)
                {
                    if (!treeNode.Data.gameObject.activeInHierarchy)
                        continue;
                    group.DoRndChoice(ref rnd);
                }
            }

            // get active connectors 
            var connectors = transform.GetComponentsInChildren<Connectors>(false);
            Assert.IsNotNull(connectors,"Can't find active connectors component after setting a configuration");
            Assert.IsTrue(connectors.Length == 1, "Wrong amount of active connectors after setting a configuration");
            Connectors = connectors[0];
            Assert.IsNotNull(Connectors);
        }

        private void BuildImpactTree()
        {
            _tree = ForEachChildrenRecursive(transform, null);

            // fill up _induction
            {
                var induction = transform.GetComponentsInChildren<Induction>(true);
                _induction = new Dictionary<string, List<Transform>>();
                foreach (var src in induction)
                    foreach (var inductionLabel in src.InductionLabels)
                        if (!_induction.ContainsKey(inductionLabel))
                            _induction.Add(inductionLabel, new List<Transform>());

                var inducedBy = transform.GetComponentsInChildren<InducedBy>(true);
                foreach (var inducedByComp in inducedBy)
                {
                    foreach (var label in inducedByComp.InductionLabels)
                    {
                        if (!_induction.ContainsKey(label))
                            Debug.LogError($"There is no induction label with name '{label}'");
                        _induction[label].Add(inducedByComp.transform);
                    }
                }
            }

            // fill up suppression
            {
                var suppression = transform.GetComponentsInChildren<Suppression>(true);
                _suppression = new Dictionary<string, List<Transform>>();
                foreach (var src in suppression)
                    foreach (var suppressionLabel in src.SuppressionLabels)
                        if (!_suppression.ContainsKey(suppressionLabel))
                            _suppression.Add(suppressionLabel, new List<Transform>());

                var suppressedBy = transform.GetComponentsInChildren<SuppressedBy>(true);
                foreach (var suppressedByComp in suppressedBy)
                {
                    foreach (var label in suppressedByComp.SuppressionLabels)
                    {
                        if (!_suppression.ContainsKey(label))
                            Debug.LogError($"There is no suppression label with name '{label}'");
                        _suppression[label].Add(suppressedByComp.transform);
                    }
                }
            }
        }

        private TreeNode<Group> ForEachChildrenRecursive(Transform iTrans, TreeNode<Group> parent)
        {
            var group = iTrans.GetComponent<Group>();
            var newTreeNode = new TreeNode<Group>(group);
            parent?.AddChild(group);

            for (int i = 0; iTrans.childCount != i; ++i)
                ForEachChildrenRecursive(iTrans.GetChild(i), newTreeNode);

            return newTreeNode;
        }

        private void Validate()
        {
            var validators = GetComponentsInChildren<BaseComponent>(true);
            foreach (var validator in validators)
            {
                if( !validator.IsValid() )
                    Debug.LogError($"Node is not valid {validator.transform.GetDebugName()}");
            }
        }

        public bool HasSuppressionLabel(string label)
        {
            Assert.IsNotNull(_suppression);
            return _suppression.ContainsKey(label);
        }

        public bool HasInductionLabel(string label)
        {
            Assert.IsNotNull(_induction);
            return _induction.ContainsKey(label);
        }

        public void Induce(string inductionLabel)
        {
            var influncedObjects =_induction[inductionLabel];
            foreach (var influncedObject in influncedObjects)
                transform.gameObject.SetActive(true);
        }

        public void Suppress(string suppressionLabel)
        {
            var influncedObjects = _suppression[suppressionLabel];
            foreach (var influncedObject in influncedObjects)
                transform.gameObject.SetActive(false);
        }
    }
}