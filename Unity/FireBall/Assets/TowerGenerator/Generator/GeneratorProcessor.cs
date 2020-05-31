using System.Collections;
using System.Collections.Generic;
using GameLib;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class GeneratorProcessor : MonoBehaviour
    {
        public enum State
        {
            Initialization,
            Generating,
            Done
        }

        public class GeneratorsWorkingState
        {
            public List<TreeNode<Blueprint.Segment>> OpenedSegments; // = new List<TreeNode<Blueprint.Segment>>(16);
        }


        protected int SeedTopology;
        protected int SeedVisual;
        protected int SeedContent;
        public GeneratorNodes GeneratorNodes { get; protected set; }

        private GeneratorsWorkingState _workingState;
        private TowerGenerator _towerGenerator;

        protected MetaProviderManager _metaProviderManager;
        
        protected StateMachine<State> _stateMachine;
        
        

        void Awake()
        {
            Init();
        }

   

        public void Generate(TowerGenerator towerGenerator, GeneratorsWorkingState workingState)
        {
            Debug.LogFormat($">>> Generate: seeds: {SeedTopology} {SeedVisual} {SeedContent}");
            _workingState = workingState;
            _towerGenerator = towerGenerator;
            StartCoroutine(GenerateStep());
        }

        void Init()
        {
            _stateMachine = new StateMachine<State>(this);
            _stateMachine.GoTo(State.Initialization, true);
            ProcessSeeds();
            _metaProviderManager = GetComponentInChildren<MetaProviderManager>();
            GeneratorNodes = GetComponentInChildren<GeneratorNodes>();
            GeneratorNodes.Init(SeedTopology);
        }

        void ProcessSeeds()
        {
            if (SeedTopology == -1)
            {
                SeedTopology = Random.Range(0, int.MaxValue);
                Debug.Log($"Using random SeedTopology: {SeedTopology}");
            }

            if (SeedVisual == -1)
            {
                SeedVisual = Random.Range(0, int.MaxValue);
                Debug.Log($"Using random SeedVisual: {SeedVisual}");
            }

            if (SeedContent == -1)
            {
                SeedContent = Random.Range(0, int.MaxValue);
                Debug.Log($"Using random SeedContent: {SeedContent}");
            }
        }

        protected IEnumerator GenerateStep()
        {
            _stateMachine.GoTo(State.Generating, true);

            
            

            // get first generator and establish a tower
            if(_workingState == null)
            {
                var container = GeneratorNodes.GetNext();
                Assert.IsNull(container, "first generator node is null");
                var cfg = container.GetComponent<GeneratorConfigBase>();
                if (cfg != null)
                {
                    Establish(cfg);
                    //var cfg = _chooserMain.GetCurrent();
                //    var firstGenerator = cfg.CreateGenerator(seed, null, this);
                //    State.ActiveGenerators.Add(firstGenerator);
                //    var firstStep = firstGenerator.EstablishTower();
                //    Assert.IsNull(_bp.Tree);
                //    Assert.IsTrue(firstStep.GeneratorCmd == GeneratorBase.TopGenStep.Cmd.SegSpawn);
                //    _bp.Tree = firstStep.Segment;

                }
                else  // establish from prototype
                {
                    var generatorProcessor = container.GetComponent<GeneratorProcessor>();
                    Assert.IsNotNull(generatorProcessor);
                    generatorProcessor.Generate(_towerGenerator, _workingState);
                }

                

                
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

        public void Establish(GeneratorConfigBase establishConfig)
        {
            _towerGenerator.Architect.Project(
                (TreeNode<SegmentArchitect.MemorySegment>)null,
                Range.One,
                Vector3.up,
                Vector3.zero,
                establishConfig.EstablishPlacement,
                null,
                null
            );

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
    }
}
