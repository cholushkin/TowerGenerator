using System;
using System.Collections.Generic;
using Assets.Plugins.Alg;
using Events;
using GameLib.DataStructures;
using GameLib.Log;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;


namespace TowerGenerator
{
    public abstract class ChunkControllerBase : MonoBehaviour
    {
        public class EventGroupChoiceDone
        {
            public EventGroupChoiceDone(Group group)
            {
                GroupChoice = group;
            }

            public Group GroupChoice { get; }
        }

        public long Seed = -1;
        public TopologyType TopologyType { get; set; }
        public ChunkConformationType ConformationType{ get; set; }


        protected TreeNode<Group> _impactTree;
        private Dictionary<string, List<Transform>> _suppression;
        private Dictionary<string, List<Transform>> _induction;
        private LogChecker Log = new LogChecker(LogChecker.Level.Verbose);
        private EventAggregator _chunkEventAggregator;


        public virtual void Init() // configure
        {
            if(Log.Verbose())
                Debug.Log("> Init");
            if (Seed == -1)
                Seed = Random.Range(0, Int32.MaxValue);
            BuildImpactTree();
        }

        public Bounds CalculateCurrentAABB(bool withMargin = true)
        {
            Assert.IsNotNull(_impactTree);
            var bounds = _impactTree.Data.gameObject.BoundBox();
            if(withMargin)
                bounds.Expand(Vector3.one * TowerGeneratorConstants.ChunkMargin * 2f);
            return bounds;
        }

        public virtual Bounds CalculateDimensionAABB()
        {
            transform.ForEachChildrenRecursive(t => t.gameObject.SetActive(t.GetComponent<DimensionsIgnorant>() == null));
            return CalculateCurrentAABB();
        }

        public void SetConfiguration()
        {
            RandomHelper rnd = new RandomHelper(Seed);
            Debug.Log(rnd.GetCurrentSeed());

            // enable all parts besides apart from hidden
            transform.ForEachChildrenRecursive(t => t.gameObject.SetActive(t.GetComponent<Hidden>() == null));

            foreach (var treeNode in _impactTree.TraverseDepthFirstPostOrder())
            {
                Group group = treeNode.Data;
                Assert.IsNotNull(group);

                if (group != null)
                {
                    if (!treeNode.Data.gameObject.activeInHierarchy)
                        continue;
                    ProcessGroupSetConfiguration(group, ref rnd);
                }
            }
        }

        public virtual void ProcessGroupSetConfiguration(Group group, ref RandomHelper rnd)
        {
            group.DoRndChoice(ref rnd);
        }

        public void EmitEventGroupChoiceDone(Group group)
        {
            _chunkEventAggregator.Publish(new EventGroupChoiceDone(group));
        }

        private void DbgPrintGroupOutcomeConfiguration(Transform groupTransform)
        {
            var comp = groupTransform.GetComponent<Group>();
            Assert.IsNotNull(comp);

            var strOutcome = $"{comp.GetType().Name}:{groupTransform.GetDebugName(false)}:";
            for (int i = 0; i < groupTransform.childCount; ++i)
            {
                strOutcome += groupTransform.GetChild(i).gameObject.activeSelf ? "V" : "X";
            }
            Debug.Log(strOutcome);
        }

        public abstract Connector[] GetConnectors();


        private void BuildImpactTree()
        {
            _chunkEventAggregator = new EventAggregator();

            // add root group
            var groupRoot = GetComponent<GroupRoot>();
            if (groupRoot == null)
                groupRoot = gameObject.AddComponent<GroupRoot>();

            _impactTree = BuildStepRecursive(groupRoot.transform, null, null);

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
            if (Log.Verbose())
                DbgPrintImpactTree();
        }

        private void DbgPrintImpactTree()
        {
            Debug.Log(">>>>> Impact tree");
            foreach (var treeNode in _impactTree.TraverseDepthFirstPostOrder())
                Debug.Log($"{treeNode.Data.transform.GetDebugName()}: level:{treeNode.Level} branch level:{treeNode.BranchLevel} ");
        }

        private TreeNode<Group> BuildStepRecursive(Transform iTrans, Transform parent, TreeNode<Group> impactParent)
        {
            var group = iTrans.GetComponent<Group>();

            var influencer = iTrans.GetComponent<IHandle<EventGroupChoiceDone>>();
            if (influencer != null)
                _chunkEventAggregator.Subscribe(influencer);

            if (group != null)
            {
                var newGroup = new TreeNode<Group>(group);
                impactParent?.AddChild(newGroup);
                impactParent = newGroup;
            }

            for (int i = 0; iTrans.childCount != i; ++i)
                BuildStepRecursive(iTrans.GetChild(i), iTrans, impactParent);

            return impactParent;
        }

#if DEBUG
        public void Validate()
        {
            BuildImpactTree();
            var validators = GetComponentsInChildren<BaseComponent>(true);
            foreach (var validator in validators)
            {
                if (!validator.IsValid())
                    Debug.LogError($"Node is not valid {validator.transform.GetDebugName()}");
            }
        }
#endif

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

        internal void Induce(string inductionLabel)
        {
            var influencedObjects = _induction[inductionLabel];
            foreach (var influencedObject in influencedObjects)
                influencedObject.gameObject.SetActive(true);
        }

        public void Suppress(string suppressionLabel)
        {
            var influencedObjects = _suppression[suppressionLabel];
            foreach (var influencedObject in influencedObjects)
                influencedObject.gameObject.SetActive(false);
        }
    }
}