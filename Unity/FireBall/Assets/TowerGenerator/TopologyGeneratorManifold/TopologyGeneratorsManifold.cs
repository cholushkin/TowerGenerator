using System.Collections;
using System.Linq;
using Assets.Plugins.Alg;
using GameLib;
using GameLib.DataStructures;
using GameLib.Log;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class TopologyGeneratorsManifold : TopologyGeneratorsManifoldBase
    {
        public enum GeneratorState
        {
            Generating,
            Paused,
            Done
        }

        public class PointerProcessor
        {
            public TreeNode<Blueprint.Segment> PointerGarbageCollector { get; private set; }
            public TreeNode<Blueprint.Segment> PointerProgress { get; private set; }
            public TreeNode<Blueprint.Segment> PointerGenerator { get; private set; }
            public float MaxDistanceProgressToGenerator;
            public float MaxDistanceProgressToGarbageCollector;
            private Blueprint _bp;

            public PointerProcessor(
                Blueprint bp,
                float maxDistanceProgressToGenerator,
                float maxDistanceProgressToGarbageCollector)
            {
                _bp = bp;
                MaxDistanceProgressToGenerator = maxDistanceProgressToGenerator;
                MaxDistanceProgressToGarbageCollector = maxDistanceProgressToGarbageCollector;
            }

            public void SetInitialPointers()
            {
                Assert.IsNotNull(_bp);
                Assert.IsNotNull(_bp.Tree);
                PointerGarbageCollector = _bp.Tree;
                PointerProgress = _bp.Tree;
                PointerGenerator = _bp.Tree;
            }

            public float DistanceYFactorProgress2Generator()
            {
                return Mathf.Abs(PointerProgress.Data.Topology.Position.y -
                                 PointerGenerator.Data.Topology.Position.y);
            }

            public void MoveProgress()
            {
                var nextTrunkNode = PointerProgress.Children.FirstOrDefault();
                if (nextTrunkNode == null)
                    return;

                // Update progress pointer
                PointerProgress = nextTrunkNode;

                // Decrease distance between PointerProgress and PointerGarbageCollector
                while (Mathf.Abs(PointerProgress.Data.Topology.Position.y - PointerGarbageCollector.Data.Topology.Position.y)
                       > MaxDistanceProgressToGarbageCollector) // y distance from PointerProgress to PointerGarbageCollector
                {
                    MoveGarbageCollectorPointer();
                }
            }

            private void MoveGarbageCollectorPointer()
            {
                PointerGarbageCollector = PointerGarbageCollector.Children.First();
            }

            public void MoveGenerator(TreeNode<Blueprint.Segment> node)
            {
                if (node == null)
                {
                    Debug.Log("no trunk segment generated in previous TopGen");
                    return;
                }

                Assert.IsTrue(node.BranchLevel == 0);
                PointerGenerator = node;
            }
        }


        [Tooltip("-1 is infinite")]
        public int CyclesCount;
        public CyclerType GeneratorCycler;

        public float MaxDistanceProgressToGenerator;
        public float MaxDistanceProgressToGarabageCollector;

        public PointerProcessor Pointers;

        public TopologyGeneratorsVisualizer TopologyVisualizer;
        public VisualBuilder VisualBuilder;


#if UNITY_EDITOR
        public bool IsGizmoDrawPointers;
#endif

        public GeneratorState State { get; protected set; }
        public LogChecker Log;
        private Blueprint _bp;

        public Transform TopologyGeneratorsTransform;
        private TopologyGeneratorBase[] _topologyGenerators;
        private Chooser<TopologyGeneratorBase> _generatorsChooser;

        public override void StartGenerate(uint seed)
        {
            StartCoroutine(GenerateTopology(seed));
        }

        private void Init(uint seed)
        {
            Debug.Log("TopologyGeneratorsManifold.Init");
            _topologyGenerators = TopologyGeneratorsTransform.GetComponents<TopologyGeneratorBase>();
            Assert.IsTrue(_topologyGenerators.Length > 0);
            // todo: pass rnd seed to chooser
            _generatorsChooser = new Chooser<TopologyGeneratorBase>(_topologyGenerators, GeneratorCycler, CyclesCount);
        }

        protected override IEnumerator GenerateTopology(uint seed)
        {
            State = GeneratorState.Generating;
            Debug.LogFormat("GenerateTopology: {0} Seed: {1}", transform.GetDebugName(), seed);

            Init(seed);

            _bp = new Blueprint();
            Pointers = new PointerProcessor(_bp, MaxDistanceProgressToGenerator, MaxDistanceProgressToGarabageCollector);

            TopologyGeneratorBase prevGenerator = null;
            TopologyGeneratorBase curGenerator;

            uint generatorChainCounter = 0;
            do
            {
                curGenerator = _generatorsChooser.GetCurrent();
                TopologyVisualizer?.ChangeGenerator(curGenerator, generatorChainCounter);

                var isFirstGenerator = prevGenerator == null; // is first generator being called
                var seedForNextGenerator = isFirstGenerator ? seed : prevGenerator.CurrentSeed;

                curGenerator.PrepareGenerate(this);

                // ----- Generate segments with current generator
                Debug.LogFormat(">>>>> Start Topology Generator: {0} with seed {1}", curGenerator, seedForNextGenerator);
                foreach (var step in curGenerator.Generate(seedForNextGenerator, prevGenerator?.CurrentState))
                {
                    if (TopologyVisualizer?.StepDelay > 0f)
                        yield return TopologyVisualizer.Wait();

                    if (_bp.Tree == null)
                    {
                        Assert.IsTrue(step.VisCmd == TopologyGeneratorBase.TopGenStep.VisualizationCmd.SegSpawn);
                        _bp.Tree = step.Segment;
                        Pointers.SetInitialPointers();
                        TopologyVisualizer?.Begin(_bp.Tree);
                        VisualBuilder?.Begin(_bp.Tree);
                    }

                    TopologyVisualizer?.Step(step);
                    VisualBuilder?.Step(step);
                    yield return null;
                } // end of chunks generating

                // --- resolve deadlock
                if (curGenerator.CurrentState.Deadlock != null)
                {
                    Debug.Log("Resolving DEADLOCK");
                    var opened = curGenerator.CurrentState.GetOpenedForGeneration();
                    if (opened.Count == 0)
                    {
                        opened = curGenerator.CurrentState.Created.Where(x =>
                            x.Data.Topology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkRoofPeak &&
                            x != curGenerator.CurrentState.Deadlock).ToList();

                        opened.AddRange(curGenerator.PrevState.Created.Where(x =>
                            x.Data.Topology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkRoofPeak &&
                            x != curGenerator.CurrentState.Deadlock));
                        Debug.Log($"switching trunk to roof {opened}");
                    }

                    Assert.IsTrue(opened.Count != 0);
                    var topMost = opened.OrderBy(x => x.Data.Topology.Position.y).Last();
                    if (topMost.Data.Topology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkRoofPeak)
                    {
                        topMost.Data.Topology.ChunkT = Blueprint.Segment.TopologySegment.ChunkType.ChunkStd;
                        topMost.Data.Topology.IsOpenedForGenerator = true;
                        var step = TopologyGeneratorBase.TopGenStep.DoStep(topMost,
                            TopologyGeneratorBase.TopGenStep.VisualizationCmd.SegChangeState);

                        if (TopologyVisualizer?.StepDelay > 0f)
                            yield return TopologyVisualizer.Wait();
                        TopologyVisualizer?.Step(step);
                        VisualBuilder?.Step(step);
                    }
                    Debug.Log($"switching trunk to {topMost}");
                    TreeNode<Blueprint.Segment>.SwitchTrunk(topMost);
                }

                // ----- Change to next in chain TopGen
                if(prevGenerator!= null)
                    Pointers.MoveGenerator(prevGenerator.CurrentState.Created.FirstOrDefault(
                        x => (x.BranchLevel == 0 &&
                              x.Data.Topology.IsOpenedForGenerator == false)
                    )); // just any of new generated segment belonged to trunk and that is stable (will not be recreated or deleted by next TopGen)

                if (Pointers.DistanceYFactorProgress2Generator() > Pointers.MaxDistanceProgressToGenerator)
                    yield return new WaitUntil(IsNeedToGenerateMore);

                prevGenerator = curGenerator;
                _generatorsChooser.Step();
                ++generatorChainCounter;

            } while (_generatorsChooser.GetCurrent() != null); // end generators chain

            // ----- finalization of the tower
            foreach (var cmd in curGenerator.Finalize(seed, curGenerator.CurrentState)) // finalizing with the global seed 
            {
                Debug.Log("Finalizing tower");
                if (TopologyVisualizer?.StepDelay > 0f)
                    yield return TopologyVisualizer.Wait();
                TopologyVisualizer?.Step(cmd);
                VisualBuilder?.Step(cmd);
                yield return null;
            }

            yield return null;
            State = GeneratorState.Done;
        }

        private bool IsNeedToGenerateMore()
        {
            return Pointers.DistanceYFactorProgress2Generator() < MaxDistanceProgressToGenerator;
        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (_bp == null)
                return;
            if (_bp.Tree == null)
                return;
            foreach (var treeNode in _bp.Tree.TraverseBreadthFirst())
            {
                // node center
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(
                    transform.TransformPoint(treeNode.Data.Topology.Position),
                    0.5f);

                // all nodes children lines

                foreach (var child in treeNode.Children)
                {
                    Gizmos.color = (child.BranchLevel == 0) ? Color.white : Color.grey;

                    var childPos = transform.TransformPoint(child.Data.Topology.Position);
                    Gizmos.DrawLine(childPos, transform.TransformPoint(treeNode.Data.Topology.Position));
                }
            }

            if (IsGizmoDrawPointers)
            {
                // _pointerGenerator
                var pointerGeneratorPos = transform.TransformPoint(Pointers.PointerGenerator.Data.Topology.Position);
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(
                    pointerGeneratorPos,
                    1.0f);
                Handles.Label(pointerGeneratorPos, "PointerGenerator");

                // _progressPointer
                var pointerProgress = transform.TransformPoint(Pointers.PointerProgress.Data.Topology.Position);
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(
                    pointerProgress,
                    1.0f);
                Handles.Label(pointerProgress, "PointerProgress");
                //Gizmos.DrawLine(pointerGeneratorPos, pointerProgress);

                // _pointerGarbageCollector
                var pointerGarbageCollectorPos =
                    transform.TransformPoint(Pointers.PointerGarbageCollector.Data.Topology.Position);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(
                    pointerGarbageCollectorPos,
                    1.0f);
                Handles.Label(pointerGarbageCollectorPos, "PointerGarbageCollector");
            }
        }
#endif
    }
}

