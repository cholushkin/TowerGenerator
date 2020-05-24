using System.Collections;
using Assets.Plugins.Alg;
using GameLib;
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
            
        }


        public int SeedTopology;
        public int SeedVisual;
        public int SeedContent;
        public GeneratorsWorkingState WorkingState;

        protected MetaProviderManager _metaProviderManager;
        public  GeneratorNodes GeneratorNodes { get; protected set; }
        protected StateMachine<State> _stateMachine;
        
        

        void Awake()
        {
            Init();
        }

        void Reset()
        {
            SeedTopology = -1;
            SeedVisual = -1;
            SeedContent = -1;
        }

        public void Generate()
        {
            Debug.LogFormat($">>> Generate: seeds: {SeedTopology} {SeedVisual} {SeedContent}");
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
            if(WorkingState == null)
            {
                //var container = _generatorNodes.GetNext();
                //Assert.IsNull(container, "first generator node is null");
                

                //var cfg = container.GetComponent<GeneratorConfigBase>();
                //if (cfg != null)
                //{
                //    var cfg = _chooserMain.GetCurrent();
                //    var firstGenerator = cfg.CreateGenerator(seed, null, this);
                //    State.ActiveGenerators.Add(firstGenerator);
                //    var firstStep = firstGenerator.EstablishTower();
                //    Assert.IsNull(_bp.Tree);
                //    Assert.IsTrue(firstStep.GeneratorCmd == GeneratorBase.TopGenStep.Cmd.SegSpawn);
                //    _bp.Tree = firstStep.Segment;

                //}
                //else
                //{
                //    // todo: establish from prototype
                //}

                
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
    }
}
