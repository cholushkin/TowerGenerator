using System.Collections;
using System.Collections.Generic;
using Assets.Plugins.Alg;
using GameLib;
using GameLib.DataStructures;
using GameLib.Random;
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
            public ExecutionFrame Frame;
            public Blueprint Blueprint;
            public List<TreeNode<Blueprint.Segment>> OpenedSegments; // = new List<TreeNode<Blueprint.Segment>>(16);

            public SegmentArchitect Architect;
            public SegmentConstructor Constructor;
            public GeneratorPointer Pointers;

            public Transform Root;
        }

        public class ExecutionFrame
        {
            public Prototype Prototype;
        }

        protected StateMachine<ProcessorState> _stateMachine;
        private Stack<ExecutionFrame> _callStack;


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
            var constructor = new SegmentConstructor(blueprint);
            var architect = new SegmentArchitect(constructor);
            _callStack = new Stack<ExecutionFrame>();
            
            var state = new State
            {
                Frame = new ExecutionFrame{Prototype = prototype},
                Blueprint = blueprint,
                OpenedSegments = new List<TreeNode<Blueprint.Segment>>(16),
                Architect = architect,
                Constructor = constructor,
                Pointers = new GeneratorPointer(blueprint),
                Root = root
            };

            _callStack.Push(state.Frame);

            return state;
        }

        int ProcessSeed(int seed)
        {
            return (seed == -1) ? Random.Range(0, int.MaxValue) : seed;
        }

        protected IEnumerator GenerateRoutine(State state)
        {
            Assert.IsNotNull(state);

            GeneratorConfigBase currentConfig = null;


            // get first generator and establish a tower
            if (state.Blueprint.Tree == null)
            {
                currentConfig = GetNextConfig(state);
                Debug.Log($"CONFIG:{currentConfig.transform.GetDebugName()}");
                Assert.IsNotNull(currentConfig, "first config is null (establish fail)");
                Establish(state, currentConfig);
                //    //var cfg = _chooserMain.GetCurrent();
                //    //    var firstGenerator = cfg.CreateGenerator(seed, null, this);
                //    //    State.ActiveGenerators.Add(firstGenerator);
                //    //    var firstStep = firstGenerator.EstablishTower();
                //    //    Assert.IsNull(_bp.Tree);
                //    //    Assert.IsTrue(firstStep.GeneratorCmd == GeneratorBase.TopGenStep.Cmd.SegSpawn);
                //    //    _bp.Tree = firstStep.Segment;

                //}
                //else  // establish from prototype
                //{
                //    var generatorProcessor = container.GetComponent<GeneratorProcessor>();
                //    Assert.IsNotNull(generatorProcessor);
                //    generatorProcessor.Generate(_towerGenerator, _workingState);
                //}
            }

            while ( (currentConfig = GetNextConfig(state)) != null)
            {
                Debug.Log($"CONFIG:{currentConfig.transform.GetDebugName()}");
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

        public void Establish(State state, GeneratorConfigBase establishConfig)
        {
            //_towerGenerator.Architect.Project(
            //    (TreeNode<SegmentArchitect.MemorySegment>)null,
            //    Range.One,
            //    Vector3.up,
            //    Vector3.zero,
            //    establishConfig.EstablishPlacement,
            //    null,
            //    null
            //);

            //    SegmentBuilder segmentBuilder = new segmen(this, _rnd.ValueInt());

            //    segmentBuilder.Project(
            //        (TreeNode<SegmentBuilder.MemorySegment>)null,
            //        Range.One,
            //        Vector3.up,
            //        Vector3.zero,
            //        Config.GetPlacementConfig(TopologyType.ChunkIsland),
            //        null,
            //        null
            //    );

            //    Assert.IsTrue(segmentBuilder.GetProjectVariantsNumber() == 1);
            //    segmentBuilder.ApplyProject(0);

            //    var segment = segmentBuilder.Build().First();
            //    Assert.IsNotNull(segment);
            //    return TopGenStep.DoStep(segment, TopGenStep.Cmd.SegSpawn);
        }



        //public virtual TopGenStep EstablishTower()
        //{
        //    SegmentBuilder segmentBuilder = new segmen(this, _rnd.ValueInt());

        //    segmentBuilder.Project(
        //        (TreeNode<SegmentBuilder.MemorySegment>)null,
        //        Range.One,
        //        Vector3.up,
        //        Vector3.zero,
        //        Config.GetPlacementConfig(TopologyType.ChunkIsland),
        //        null,
        //        null
        //    );

        //    Assert.IsTrue(segmentBuilder.GetProjectVariantsNumber() == 1);
        //    segmentBuilder.ApplyProject(0);

        //    var segment = segmentBuilder.Build().First();
        //    Assert.IsNotNull(segment);
        //    return TopGenStep.DoStep(segment, TopGenStep.Cmd.SegSpawn);
        //}


        GeneratorConfigBase GetNextConfig(State state)
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

            var container = state.Frame.Prototype.GeneratorNodes.GetNext();
            if (container == null) // out of genNodes on this prototype
            {
                state.Frame = StepOut();
                // switch prototype (return on a level back)
                if (state.Frame == null)
                    return null; // no configs anymore - need to finalize construction
                return GetNextConfig(state);
            }

            var cfg = container.GetComponent<GeneratorConfigBase>();
            if (cfg)
            {
                cfg.OnProcessorEnter();
                return cfg;
            }

            state.Frame = StepIn(container.GetComponent<Prototype>());
            return GetNextConfig(state);
        }

       
    }
}
