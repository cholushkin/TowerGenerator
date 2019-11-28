using System;
using System.Linq;
using Assets.Plugins.Alg;
using Boo.Lang;
using GameLib;
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
        //public Bounds BBMax;
        //public Bounds BBMin;
        public long Seed = -1;

        public GroupStack GroupSizeStack;

        public void Init()
        {
            //if (Seed == -1)
            //    Seed = Random.Range(0, Int32.MaxValue);
            GroupSizeStack = GetComponentInChildren<GroupStack>();
            Assert.IsNotNull(GroupSizeStack, $"{gameObject.transform.GetDebugName()}");
        }

        //public class GroupVariant
        //{
        //    public Group Group;
        //    public int Variant;
        //    public int Index;

        //    public override string ToString()
        //    {
        //        return $"{Group.name}_{Variant}({Index})";
        //    }
        //}

        //[ContextMenu("GetAllPermutations")]
        //public void GetAllPermutations()
        //{
        //    // get all groups (except UserGroup)
        //    var groups = GetComponentsInChildren<Group>().Where(x => x is GroupUser == false);

        //    List<GroupVariant> combinations = new List<GroupVariant>();

        //    // get list of all variants
        //    var index = 0;
        //    foreach (var group in groups)
        //    {
        //        for (int i = 0; i < group.GetNumberOfPermutations(); i++)
        //        {
        //            var gVar = new GroupVariant
        //            {
        //                Group = group,
        //                Variant = i,
        //                Index = index++
        //            };
        //            combinations.Push(gVar);
        //            Debug.Log($"{gVar}");
        //        }
        //    }
        //    Debug.Log($"Amount of all variants : {index}");
        //    Debug.Log($"{groups.ToArray().Count()}");
        //    Debug.Log($"Amount of combinations with that variants: {GetAmountOfGroupVarCombinations(index,8)}");
        //}

        //public ulong GetAmountOfGroupVarCombinations(int n, int m)
        //{
        //    return (Numbers.Factorial(n) / (Numbers.Factorial(n - m) * Numbers.Factorial(m)));
        //}

        //string NextSet(int* a, int n, int m)
        //{
        //    int k = m;
        //    for (int i = k - 1; i >= 0; --i)
        //        if (a[i] < n - k + i + 1)
        //        {
        //            ++a[i];
        //            for (int j = i + 1; j < k; ++j)
        //                a[j] = a[j - 1] + 1;
        //            return true;
        //        }
        //    return false;
        //}


        public void BuildImpactTree()
        {
            ForEachChildrenRecursive(transform, null);
        }

        public Bounds CalculateBB()
        {
            if (_tree == null)
                BuildImpactTree();
            var bb = _tree.Data.Transform.gameObject.BoundBox();
            return bb;
        }

        //public Bounds CalculateBBMax()
        //{
        //    foreach (var treeNode in _tree.TraverseDepthFirstPostOrder())
        //    {
        //        if (treeNode.Data.Transform.GetComponent<Connectors>() != null)
        //            treeNode.Data.Transform.gameObject.SetActive(false);
        //        else
        //            treeNode.Data.Transform.gameObject.SetActive(true);
        //    }

        //    BBMax = _tree.Data.Transform.gameObject.BoundBox();
        //    BBMax.center = Vector3.zero;
        //    //Debug.Log($"BBmax:{BBMax}");
        //    return BBMax;
        //}

        //[ContextMenu("CalculateBBSize")]
        //public Vector3 CalculateBBSize()
        //{
        //    if (_tree == null)
        //        BuildImpactTree();
        //    BBMax = _tree.Data.Transform.gameObject.BoundBox();
        //    BBMax.center = Vector3.zero;
        //    //Debug.Log($"BBmax:{BBMax}");
        //    return BBMax.size;
        //}


        //public bool SetMaximizedFitRndConfiguration(Vector3 maxBBSize)
        //{
        //    var origSeed = Seed;

        //    const int iterations = 32;
        //    float maxVolume = 0;
        //    long fitSeed = -1;
        //    for (int i = 0; i < iterations; i++)
        //    {
        //        var s = Seed;
        //        SetRndConfiguration();
        //        var bbxize = CalculateBBSize();
        //        var volume = bbxize.x * bbxize.y * bbxize.z;
        //        //Debug.Log($"Volume {volume}");

        //        var isInside = bbxize.x <= maxBBSize.x && bbxize.y <= maxBBSize.y && bbxize.z <= maxBBSize.z;

        //        if (volume > maxVolume && isInside)
        //        {
        //            maxVolume = volume;
        //            fitSeed = s;
        //        }
        //    }

        //    if (fitSeed == -1)
        //    {
        //        Debug.Log($"Can't fit! Using SetMinimizedRndConfiguration with original seed {origSeed}");
        //        Seed = origSeed;
        //        SetMinimizedRndConfiguration();
        //        var bbxize = CalculateBBSize();
        //        var isInside = bbxize.x <= maxBBSize.x && bbxize.y <= maxBBSize.y && bbxize.z <= maxBBSize.z;
        //        Debug.LogError(
        //            $"Can't fit even with minimal cfg resulting bb:{bbxize} in requested bb:{maxBBSize} - obj:{gameObject}");
        //        SetRndConfiguration();
        //        return false;
        //    }

        //    //Debug.Log($"Max Volume {maxVolume} seed for it {fitSeed}");
        //    Seed = fitSeed;
        //    SetRndConfiguration();
        //    return true;
        //}


        // todo: remove me
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
            //Debug.Log(rnd.GetCurrentSeed());

            // enable all parts
            transform.ForEachChildrenRecursive(t => t.gameObject.SetActive(true));

            if (_tree == null)
                BuildImpactTree();

            // all possible hosts works first
            var hosts = GetComponentsInChildren<GroupStack>();
            foreach (var groupStack in hosts)
            {
                if (minBB)
                    groupStack.DoRndMinimalChoice(ref rnd);
                else
                    groupStack.DoRndChoice(ref rnd);
            }

            foreach (var treeNode in _tree.TraverseDepthFirstPostOrder())
            {
                var group = treeNode.Data.Group;
                if (group != null)
                {
                    if (!treeNode.Data.Transform.gameObject.activeInHierarchy)
                        continue;

                    var groupIsActual = true;
                    if(group.Host != null)
                        groupIsActual = group.Host.LayerIndexSelected <= group.PropagatedTo;

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


        //public void CalculateBBMin()
        //{
        //    foreach (var treeNode in _tree.TraverseBreadthFirst())
        //    {
        //    }
        //}

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