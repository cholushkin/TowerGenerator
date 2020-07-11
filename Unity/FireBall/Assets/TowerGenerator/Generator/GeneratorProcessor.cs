using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Plugins.Alg;
using GameLib;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class GeneratorProcessor : MonoBehaviour
    {
        public enum ProcessorState
        {
            Initialization,
            Generating,
            Done
        }

        public class State
        {
            public Blueprint Blueprint;
            public List<TreeNode<Blueprint.Segment>> OpenedSegments; // = new List<TreeNode<Blueprint.Segment>>(16);
            public List<TreeNode<Blueprint.Segment>> DeadlockSegments; // = new List<TreeNode<Blueprint.Segment>>(16);

            // public SegmentArchitect Architect; moved to generator
            //public SegmentConstructor Constructor;
            public int ConfigCounter;
            public GeneratorPointer Pointers;
        }

        public class ExecutionFrame
        {
            public Prototype Prototype;
        }

        public ChunkVisualConstructor Constructor;

        protected StateMachine<ProcessorState> _stateMachine;
        private GeneratorConfigBase _currentConfig;
        private Queue<TreeNode<Blueprint.Segment>> _lastSegments;
        private ExecutionFrame _frame;
        private Stack<ExecutionFrame> _callStack;
        private Transform _towerRoot;
#if DEBUG
        private State _state;
#endif

        private const int LastSegmentsQueueSize = 4;

        public void StartGenerate(Prototype prototype, Transform root)
        {
            Debug.LogFormat($">>> StartGenerate");

            _stateMachine = new StateMachine<ProcessorState>(this);
            _stateMachine.GoTo(ProcessorState.Initialization, true);

            var state = Init(prototype, root);

            Generate(state);
        }

        public void Generate(State state)
        {
            _stateMachine.GoTo(ProcessorState.Generating, true);
            StartCoroutine(GenerateRoutine(state));
        }

        State Init(Prototype prototype, Transform root)
        {
            var blueprint = new Blueprint();
            _callStack = new Stack<ExecutionFrame>();
            _frame = new ExecutionFrame { Prototype = prototype };
            _towerRoot = root;
            _lastSegments = new Queue<TreeNode<Blueprint.Segment>>();

            var pointers = new GeneratorPointer(blueprint);

            var state = new State
            {
                Blueprint = blueprint,
                OpenedSegments = new List<TreeNode<Blueprint.Segment>>(16),
                DeadlockSegments = new List<TreeNode<Blueprint.Segment>>(16),
                Pointers = pointers
            };

            Constructor.Init(blueprint, _towerRoot, pointers);

            _callStack.Push(_frame);
#if DEBUG
            _state = state;
#endif
            return state;
        }

        protected IEnumerator GenerateRoutine(State state)
        {
            Assert.IsNotNull(state);

            GeneratorConfigBase currentConfig = null;


            // get first generator and establish a tower
            if (state.Blueprint.Tree == null)
            {
                currentConfig = GetNextConfig();
                Debug.Log($"CONFIG:{currentConfig.transform.GetDebugName()}");
                Assert.IsNotNull(currentConfig, "first config is null (establish fail)");
                Establish(state, currentConfig);
                _lastSegments.Enqueue(state.OpenedSegments.First());
                yield return null;
            }

            while ((currentConfig = GetNextConfig()) != null)
            {
                Debug.Log($"CONFIG:{currentConfig.transform.GetDebugName()}");
                var currentGenerator = currentConfig.CreateGenerator();
                yield return null;

                const int MaxIterationForGenerator = 16;
                for (int i = 0; i < MaxIterationForGenerator; ++i)
                {
                    currentGenerator.Generate(state, i);

                    // update last segments
                    var trunkLastSegment = state.OpenedSegments.OrderBy(x => x.BranchLevel).First();
                    _lastSegments.Enqueue(trunkLastSegment);
                    while (_lastSegments.Count > LastSegmentsQueueSize)
                        _lastSegments.Dequeue();
                    Assert.IsNotNull(trunkLastSegment);
                    Assert.IsTrue(trunkLastSegment.BranchLevel == 0);

                    // update pointers
                    state.Pointers.SetPointerGeneratorTopTrunk( trunkLastSegment, _lastSegments.Peek());

                    yield return new WaitUntil(()=>state.Pointers.IsNeededToGenerateMore);

                    if (state.DeadlockSegments.Count != 0)
                    {
                        //ResolveDeadlocks
                        yield return null;
                    }

                    yield return null;
                    if (currentGenerator.Done())
                        break;
                }



                //currentGenerator.

            }

            // after first step initialization
            //if (TopologyVisualizer?.StepDelay > 0f)
            //    yield return TopologyVisualizer.Wait();
            //Pointers.SetInitialPointers();
            //TopologyVisualizer?.Begin(_bp.Tree);
            //TopologyVisualizer?.ChangeGenerator(State.ActiveGenerators.First());
            //VisualBuilder?.Begin(_bp.Tree);
            //yield return null;


            //bool isWaitingForBranchFinalization = false;
            //do
            //{
            //    // generate
            //    foreach (var activeGenerator in State.ActiveGenerators)
            //    {
            //        foreach (var step in activeGenerator.GenerateTower())
            //        {
            //            TopologyVisualizer?.Step(step);
            //        }
            //        activeGenerator.State.Iteration++;
            //    }

            //    var index = 0;
            //    List<GeneratorBase> newActiveGenerators = new List<GeneratorBase>();
            //    foreach (var activeGenerator in State.ActiveGenerators)
            //    {
            //        if (index == 0)
            //        {
            //            //if( activeGenerator.State.IsStillGeneratingTrunk )
            //            //    newActiveGenerators.Add(activeGenerator);
            //            //else
            //            //{
            //            //    _genConfigChooser.Step();
            //            //    var cfg = _genConfigChooser.GetCurrent();
            //            //    if (cfg != null)
            //            //    {
            //            //        var generator = cfg.CreateGenerator(
            //            //            activeGenerator.GetCurrentSeed(),
            //            //            activeGenerator.State.GetOpenedTrunkNode(),
            //            //            this
            //            //        );
            //            //        newActiveGenerators.Add(generator);
            //            //        TopologyVisualizer?.ChangeGenerator(generator);
            //            //    }
            //            //    else // no more generators in chain, but we still need to wait for branch generators to complete
            //            //    {
            //            //        // finalize trunk
            //            //        var step = activeGenerator.FinalizeTrunk();
            //            //        TopologyVisualizer?.Step(step);
            //            //        isWaitingForBranchFinalization = true;
            //            //    }
            //            //}
            //            continue;
            //        }
            //        //Assert.IsFalse(activeGenerator.State.IsStillGeneratingTrunk);
            //        //if(activeGenerator.State.GetOpenedForGeneration().Any())
            //        //    newActiveGenerators.Add(activeGenerator);
            //        ++index;
            //    }

            //    if (State.ActiveGenerators.Count == 0)
            //        State.Status = TopologyGeneratorsManifold.ManifoldState.ManifoldStatus.Done;
            //} while (State.Status != TopologyGeneratorsManifold.ManifoldState.ManifoldStatus.Done);

            yield return null;
        }

        private void OnNewConfigEnter(State state, GeneratorConfigBase newConfig, GeneratorConfigBase prevConfig)
        {
            _currentConfig = newConfig;
            state.ConfigCounter++;
        }

        public void Establish(State state, GeneratorConfigBase config)
        {
            var architect = new SegmentArchitect(
                config.SeedTopology,  config.SeedContent, config.SeedVisual,
                state.Pointers.PointerGeneratorStable, config,
                TopologyType.ChunkFoundation, TopologyType.Undefined, TopologyType.Undefined);

            // override default placement config picker
            {
                // try to get specific foundation first for this config
                var foundationPlacementCfg = config.GetPlacementConfig(TopologyType.ChunkFoundation, SpecificsStringConstants.Establishment);

                // then get just regular foundation
                if (foundationPlacementCfg == null)
                {
                    foundationPlacementCfg = config.GetPlacementConfig(TopologyType.ChunkFoundation);
                    Assert.IsNotNull(foundationPlacementCfg);
                }

                architect.PlacementConfigProvider = (segRelativePos, segIndex) => foundationPlacementCfg;
            }

            architect.MakeProjects(
                null,
                Range.One,
                Vector3.zero, Vector3.up
            );

            Assert.IsTrue(architect.GetProjectVariantsNumber() == 1);
            var project = architect.GetProject(0, out var openedSeg);

            state.Blueprint.AddSubtree(null, Vector3.up, project);
            state.OpenedSegments.Add(openedSeg);

            state.Pointers.SetInitialPointers();
        }

        GeneratorConfigBase GetNextConfig()
        {
            ExecutionFrame StepOut()
            {
                Assert.IsTrue(_callStack.Count > 0);
                _callStack.Pop();
                if (_callStack.Count == 0)
                    return null;
                return _callStack.Peek();
            }

            ExecutionFrame StepIn(Prototype prototype)
            {
                Assert.IsNotNull(prototype);
                var newFrame = new ExecutionFrame { Prototype = prototype };
                _callStack.Push(newFrame);
                newFrame.Prototype.GeneratorNodes.OnProcessorEnter();
                return newFrame;
            }

            var container = _frame.Prototype.GeneratorNodes.GetNext();
            if (container == null) // out of genNodes on this prototype
            {
                _frame = StepOut();
                // switch prototype (return on a level back)
                if (_frame == null)
                    return null; // no configs anymore - need to finalize construction
                return GetNextConfig();
            }

            var cfg = container.GetComponent<GeneratorConfigBase>();
            if (cfg)
            {
                cfg.OnProcessorEnter();
                OnNewConfigEnter(_state, cfg, _currentConfig);
                return cfg;
            }

            _frame = StepIn(container.GetComponent<Prototype>());
            return GetNextConfig();
        }


#if DEBUG
        void OnDrawGizmos()
        {
            if (_state == null)
                return;

            foreach (var treeNode in _state.Blueprint.Tree.TraverseBreadthFirst())
            {
                //    // node center
                //    Gizmos.color = Color.red;
                //    Gizmos.DrawSphere(
                //        transform.TransformPoint(treeNode.Data.Topology.Geometry.Position),
                //        0.5f);

                Gizmos.DrawWireCube(
                    transform.TransformPoint(treeNode.Data.Topology.Geometry.Bounds.center + _towerRoot.localPosition),
                    treeNode.Data.Topology.Geometry.Bounds.size);
                var labelPos = transform.TransformPoint(treeNode.Data.Topology.Geometry.Bounds.center + _towerRoot.localPosition);
                //    Gizmos.color = Color.black;
                //    Gizmos.DrawWireSphere(
                //        pointerGeneratorPos,
                //        1.0f);
                Handles.Label(labelPos, treeNode.Data.ToString(), new GUIStyle { fontSize = 10 });

                //    Gizmos.color = Color.gray;
                //    Gizmos.DrawWireCube(
                //        transform.TransformPoint(treeNode.Data.Topology.Geometry.Position),
                //        treeNode.Data.Topology.Geometry.AspectRatio - TowerGeneratorConstants.ConnectorMargin);


                //    // all nodes children lines
                //    foreach (var child in treeNode.Children)
                //    {
                //        Gizmos.color = (child.BranchLevel == 0) ? Color.white : Color.grey;

                //        var childPos = transform.TransformPoint(child.Data.Topology.Geometry.Position);
                //        Gizmos.DrawLine(childPos, transform.TransformPoint(treeNode.Data.Topology.Geometry.Position));
                //    }
                //}

                //if (IsGizmoDrawPointers)
                //{
                //    // _pointerGenerator
                //    var pointerGeneratorPos = transform.TransformPoint(Pointers.PointerGenerator.Data.Topology.Geometry.Position);
                //    Gizmos.color = Color.black;
                //    Gizmos.DrawWireSphere(
                //        pointerGeneratorPos,
                //        1.0f);
                //    Handles.Label(pointerGeneratorPos, "PointerGenerator");


                //    // _pointerStable
                //    var pointerStablePos = transform.TransformPoint(Pointers.PointerStable.Data.Topology.Geometry.Position);
                //    Gizmos.color = Color.black;
                //    Gizmos.DrawWireSphere(
                //        pointerStablePos,
                //        1.0f);
                //    Handles.Label(pointerStablePos, "PointerStable");

                //    // _progressPointer
                //    var pointerProgress = transform.TransformPoint(Pointers.PointerProgress.Data.Topology.Geometry.Position);
                //    Gizmos.color = Color.white;
                //    Gizmos.DrawWireSphere(
                //        pointerProgress,
                //        1.0f);
                //    Handles.Label(pointerProgress, "PointerProgress");
                //    //Gizmos.DrawLine(pointerGeneratorPos, pointerProgress);

                //    // _pointerGarbageCollector
                //    var pointerGarbageCollectorPos =
                //        transform.TransformPoint(Pointers.PointerGarbageCollector.Data.Topology.Geometry.Position);
                //    Gizmos.color = Color.yellow;
                //    Gizmos.DrawWireSphere(
                //        pointerGarbageCollectorPos,
                //        1.0f);
                //    Handles.Label(pointerGarbageCollectorPos, "PointerGarbageCollector");
            }
        }
#endif
    }
}
