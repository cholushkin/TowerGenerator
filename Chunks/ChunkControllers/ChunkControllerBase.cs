using System;
using System.Collections.Generic;
using GameLib.Alg;
using GameLib.Log;
using GameLib.Random;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using VitalRouter;
using Random = GameLib.Random.Random;


namespace TowerGenerator
{
    public class ChunkControllerBase : MonoBehaviour
    {
        public class EventNodeActiveStateChanged : ICommand
        {
            public GameObject Node;
        }

        // Private fields accessor for debug purposes
        public class DebugChunkControllerBase
        {
            public DebugChunkControllerBase(ChunkControllerBase chunkController)
            {
                _dbg = chunkController;
            }

            public string GetChunkControllerName() => _dbg.name;
            
            public Dictionary<string, List<Transform>> GetSuppressionDictionary() => _dbg._suppression;

            public Dictionary<string, List<Transform>> GetInductionDictionary() => _dbg._induction;

            private ChunkControllerBase _dbg;
        }

        // Tags used for specifying chunk topology
        public const string ChunkPeekTag = "ChunkPeek";
        public const string ChunkStandardTag = "ChunkStandard";
        public const string ChunkBasementTag = "ChunkBasement";
        public const string ChunkIslandAndBasementTag = "ChunkIslandAndBasement";
        public const string ChunkIslandTag = "ChunkIsland";
        public const string ChunkSideEarTag = "ChunkSideEar";
        public const string ChunkBottomEarTag = "ChunkBottomEar";
        public const string ChunkTopEarTag = "ChunkTopEar";

        // note: one chunk must belong to only one ChunkController, but you could request multiple ChunkController types by combining flags
        [Flags]
        public enum ChunkController
        {
            Undefined = 0,
            BasicChunkController = 1,
            WaveFuncCollapseChunkController = 2,
            GrowingChunkController = 4,
            MarchingCubesChunkController = 8,
        }

        public long Seed = -1;
        public uint RngState;
        public MetaBase Meta;
        [ReadOnly] public string ImportBasedOnHash;


        protected TreeNode<Group> _impactTree;
        private Dictionary<string, List<Transform>> _suppression;
        private Dictionary<string, List<Transform>> _induction;
        public LogChecker Log = new LogChecker(LogChecker.Level.Verbose);


        public virtual void Init() // configure
        {
            if (Log.Verbose())
                Debug.Log("> Init");
            if (Log.Normal())
                Debug.Log($"{transform.GetDebugName()} Seed {Seed}");
            BuildImpactTree();
            InitializeInduction();
            InitializeSuppression();
        }

        public TreeNode<Group> GetImpactTree()
        {
            return _impactTree;
        }

        public Bounds CalculateCurrentAABB(bool withMargin = true)
        {
            Assert.IsNotNull(_impactTree);
            var bounds = _impactTree.Data.gameObject.BoundBox();
            if (withMargin)
                bounds.Expand(Vector3.one * Meta.ChunkMargin * 2f);
            return bounds;
        }

        // todo: move to editor only
        public virtual Bounds CalculateDimensionAABB()
        {
            transform.ForEachChildrenRecursive(t => t.gameObject.SetActive(t.GetComponent<DimensionsIgnorant>() == null));
            return CalculateCurrentAABB();
        }

        // Sets random state of the chunk using current Seed
        public void SetConfiguration()
        {
            if(_impactTree == null)
                Init();
            
            // Randomize seed if Seed==-1
            if (Seed == -1)
                Seed = RandomHelper.Rng.ValueInt();
            Random rnd = RandomHelper.CreateRandomNumberGenerator((uint)Seed);
            // Override state if it's not a zero state
            if (RngState != 0)
                rnd.SetState(RngState);

            // Set default state of the tree
            InitializeTreeState();

            // Set initial configuration of all groups
            
            foreach (var treeNode in _impactTree.TraverseDepthFirstPreOrder())
            {
                treeNode.Data.SetRandomState(rnd, false);
            }

            // Notify ChunkController
            SetNodeActiveState(_impactTree.Data.transform, true);
            
            // set current state
            RngState = rnd.GetState();
        }

        public Connector[] GetActiveConnectors()
        {
            throw new NotImplementedException();
        }

        private void BuildImpactTree()
        {
            // Add root group if needed
            var groupRoot = GetComponent<Group>();
            if (groupRoot == null)
                groupRoot = gameObject.AddComponent<GroupRoot>();

            // Create tree
            _impactTree = BuildStepRecursive(groupRoot.transform, null, null);
        }

        private void InitializeSuppression()
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

        private void InitializeInduction()
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

        private void InitializeTreeState()
        {
            // Default state:
            // All groups items are disabled
            // All non group items are enabled
            // All hidden objects are disabled

            // Enable all parts apart from Hidden
            transform.ForEachChildrenRecursive(t => t.gameObject.SetActive(t.GetComponent<Hidden>() == null));

            foreach (var treeNode in _impactTree.TraverseDepthFirstPreOrder())
            {
                // Assign items, disable all group items
                Group group = treeNode.Data;
                group.Initialize(); 
                group.DisableAllItems();
            }
        }
                      
        private TreeNode<Group> BuildStepRecursive(Transform iTrans, Transform parent, TreeNode<Group> impactParent)
        {
            var group = iTrans.GetComponent<Group>();

            if (group != null)
            {
                group.ChunkController = this;
                var newGroup = new TreeNode<Group>(group);
                impactParent?.AddChild(newGroup);
                impactParent = newGroup;
            }

            for (int i = 0; iTrans.childCount != i; ++i)
                BuildStepRecursive(iTrans.GetChild(i), iTrans, impactParent);

            return impactParent;
        }

        public void SetNodeActiveState(Transform node, bool newActiveState)
        {
            if (newActiveState) // new suppressors/inductor revealed - let them work
            {
                node.gameObject.SetActive(true); // enable
                if (node.gameObject.activeInHierarchy == false) // set active to obscured node
                    return;

                var suppressions = node.GetComponentsInChildren<Suppression>(false); // get all visible suppression nodes after enabling starting from the node
                var inductions = node.GetComponentsInChildren<Induction>(false);
                
                foreach (var suppression in suppressions)
                    foreach (var suppressionLabel in suppression.SuppressionLabels)
                        Suppress(suppressionLabel);

                foreach (var induction in inductions)
                    foreach (var inductionLabel in induction.InductionLabels)
                        Induce(inductionLabel);
            }
            else // active suppressors disabled - need to reverse suppress
            {
                if (node.gameObject.activeInHierarchy == false) // set not active to obscured node
                {
                    node.gameObject.SetActive(false); // disable
                    return;
                }
                var suppressions = node.GetComponentsInChildren<Suppression>(false); // get all visible suppression nodes before disabling starting from the node
                var inductions = node.GetComponentsInChildren<Induction>(false);

                node.gameObject.SetActive(false); // disable

                foreach (var suppression in suppressions)
                    foreach (var suppressionLabel in suppression.SuppressionLabels)
                        Suppress(suppressionLabel, true);
                
                foreach (var induction in inductions)
                    foreach (var inductionLabel in induction.InductionLabels)
                        Induce(inductionLabel, true);
            }

            VitalRouter.Router.Default.PublishAsync(new EventNodeActiveStateChanged { Node = node.gameObject });
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

        internal void Induce(string inductionLabel, bool reverse = false)
        {
            var influencedObjects = _induction[inductionLabel];
            foreach (var influencedObject in influencedObjects)
                SetNodeActiveState(influencedObject, !reverse);
        }

        public void Suppress(string suppressionLabel, bool reverse = false)
        {
            var influencedObjects = _suppression[suppressionLabel];
            foreach (var influencedObject in influencedObjects)
                SetNodeActiveState(influencedObject, reverse);
        }
    }
}