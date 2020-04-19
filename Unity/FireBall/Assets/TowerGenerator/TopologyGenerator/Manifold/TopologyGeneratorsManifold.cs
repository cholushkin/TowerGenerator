using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Plugins.Alg;
using GameLib;
using GameLib.DataStructures;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class TopologyGeneratorsManifold : TopologyGeneratorsManifoldBase
    {
        public class ManifoldState
        {
            public enum ManifoldStatus
            {
                Initialization,
                Generating,
                Done
            }

            public List<GeneratorBase> ActiveGenerators;
            public ManifoldStatus Status;
        }

        // main configs
        [Header("Main configs")]
        [Tooltip("-1 is infinite")]
        public int MainCfgChooserCyclesCount;
        public CyclerType MainCfgCycler;
        public Transform MainConfigs;

        // branch finalizers configs
        [Header("Branch finalizer configs")]
        [Tooltip("-1 is infinite")]
        public int BranchCfgCyclesCount;
        public CyclerType BranchCfgCycler;
        public Transform BranchConfigs;

        [Space(12)]
        public TopologyGeneratorsVisualizer TopologyVisualizer;
        public VisualBuilder VisualBuilder;

#if UNITY_EDITOR
        public bool IsGizmoDrawPointers;
#endif

        public ManifoldState State { get; protected set; }
        
        // main cfg chooser
        private GeneratorConfigBase[] _configsMain;
        private Chooser<GeneratorConfigBase> _chooserMain;

        // branch cfg chooser
        private GeneratorConfigBase[] _configsBranches;
        private Chooser<GeneratorConfigBase> _chooserBranch;




        private void Init(uint seed)
        {
            Debug.Log("TopologyGeneratorsManifold.Init");

            // main chooser init
            _configsMain = MainConfigs.GetComponents<GeneratorConfigBase>();
            Assert.IsTrue(_configsMain.Length > 0);
            _chooserMain = new Chooser<GeneratorConfigBase>(_configsMain, MainCfgCycler, seed, MainCfgChooserCyclesCount);

            // branch chooser init
            if (BranchConfigs != null)
            {
                _configsBranches = BranchConfigs.GetComponents<GeneratorConfigBase>();
                Assert.IsTrue(_configsBranches.Length > 0);
                _chooserBranch =
                    new Chooser<GeneratorConfigBase>(_configsBranches, BranchCfgCycler, seed, BranchCfgCyclesCount);
            }
            else
            {
                Debug.LogFormat($"No branch configs specified found. Manifold '{transform.GetDebugName(false)}' will use main configs for branches");
                _chooserBranch = _chooserMain;
            }
        }

        public override void StartGenerate(uint seed)
        {
            Debug.LogFormat($"StartGenerate: seed:{seed}, transform:{transform.GetDebugName()}");
            StartCoroutine(GenerateTopology(seed));
        }

        protected override IEnumerator GenerateTopology(uint seed)
        {
            State.Status = ManifoldState.ManifoldStatus.Generating;
            
            Init(seed);
            _bp = new Blueprint();
            Pointers = new PointerProcessor(_bp, MaxDistanceProgressToGenerator, MaxDistanceProgressToGarabageCollector);

            // get first generator and establish a tower
            {
                var cfg = _chooserMain.GetCurrent();
                var firstGenerator = cfg.CreateGenerator(seed, null, this);
                State.ActiveGenerators.Add(firstGenerator);
                var firstStep = firstGenerator.EstablishTower();
                Assert.IsNull(_bp.Tree);
                Assert.IsTrue(firstStep.GeneratorCmd == GeneratorBase.TopGenStep.Cmd.SegSpawn);
                _bp.Tree = firstStep.Segment;
            }
            
            // after first step initialization
            if (TopologyVisualizer?.StepDelay > 0f)
                yield return TopologyVisualizer.Wait();
            Pointers.SetInitialPointers();
            TopologyVisualizer?.Begin(_bp.Tree);
            TopologyVisualizer?.ChangeGenerator(State.ActiveGenerators.First());
            VisualBuilder?.Begin(_bp.Tree);
            yield return null;


            bool isWaitingForBranchFinalization = false;
            do
            {
                // generate
                foreach (var activeGenerator in State.ActiveGenerators)
                {
                    foreach (var step in activeGenerator.GenerateTower())
                    {
                        TopologyVisualizer?.Step(step);
                    }
                    activeGenerator.State.Iteration++;
                }

                var index = 0;
                List<GeneratorBase> newActiveGenerators = new List<GeneratorBase>();
                foreach (var activeGenerator in State.ActiveGenerators)
                {
                    if (index == 0)
                    {
                        //if( activeGenerator.State.IsStillGeneratingTrunk )
                        //    newActiveGenerators.Add(activeGenerator);
                        //else
                        //{
                        //    _genConfigChooser.Step();
                        //    var cfg = _genConfigChooser.GetCurrent();
                        //    if (cfg != null)
                        //    {
                        //        var generator = cfg.CreateGenerator(
                        //            activeGenerator.GetCurrentSeed(),
                        //            activeGenerator.State.GetOpenedTrunkNode(),
                        //            this
                        //        );
                        //        newActiveGenerators.Add(generator);
                        //        TopologyVisualizer?.ChangeGenerator(generator);
                        //    }
                        //    else // no more generators in chain, but we still need to wait for branch generators to complete
                        //    {
                        //        // finalize trunk
                        //        var step = activeGenerator.FinalizeTrunk();
                        //        TopologyVisualizer?.Step(step);
                        //        isWaitingForBranchFinalization = true;
                        //    }
                        //}
                        continue;
                    }
                    //Assert.IsFalse(activeGenerator.State.IsStillGeneratingTrunk);
                    //if(activeGenerator.State.GetOpenedForGeneration().Any())
                    //    newActiveGenerators.Add(activeGenerator);
                    ++index;
                }

                if (State.ActiveGenerators.Count == 0)
                    State.Status = ManifoldState.ManifoldStatus.Done;
            } while (State.Status != ManifoldState.ManifoldStatus.Done);

            yield return null;
        }

        //protected override IEnumerator GenerateTopology(uint seed)
        //{
        //    State = GeneratorState.Generating;
        //    Debug.LogFormat("GenerateTopology: {0} Seed: {1}", transform.GetDebugName(), seed);

        //    Init(seed);

        //    _bp = new Blueprint();
        //    Pointers = new PointerProcessor(_bp, MaxDistanceProgressToGenerator, MaxDistanceProgressToGarabageCollector);

        //    TopologyGeneratorBase prevGenerator = null;
        //    TopologyGeneratorBase curGenerator;

        //    uint generatorChainCounter = 0;
        //    do
        //    {
        //        curGenerator = _generatorsChooser.GetCurrent();
        //        TopologyVisualizer?.ChangeGenerator(curGenerator, generatorChainCounter);

        //        var isFirstGenerator = prevGenerator == null; // is first generator being called
        //        var seedForNextGenerator = isFirstGenerator ? seed : prevGenerator.CurrentSeed;

        //        curGenerator.PrepareGenerate(this);

        //        // ----- Generate segments with current generator
        //        Debug.LogFormat(">>>>> Start Topology Generator: {0} with seed {1}", curGenerator, seedForNextGenerator);
        //        foreach (var step in curGenerator.Generate(seedForNextGenerator, prevGenerator?.CurrentState))
        //        {
        //            if (TopologyVisualizer?.StepDelay > 0f)
        //                yield return TopologyVisualizer.Wait();

        //            if (_bp.Tree == null)
        //            {
        //                Assert.IsTrue(step.VisCmd == TopologyGeneratorBase.TopGenStep.VisualizationCmd.SegSpawn);
        //                _bp.Tree = step.Segment;
        //                Pointers.SetInitialPointers();
        //                TopologyVisualizer?.Begin(_bp.Tree);
        //                VisualBuilder?.Begin(_bp.Tree);
        //            }

        //            TopologyVisualizer?.Step(step);
        //            yield return null;
        //        } // end of chunks generating

        //        Debug.Log("Generator work finish");
        //        Debug.Log($"Opened: {curGenerator.CurrentState.GetOpenedForGeneration().Count}");
        //        Debug.Log($"prev state opened: {curGenerator?.PrevState?.GetOpenedForGeneration().Count}");

        //        if(curGenerator?.PrevState != null)
        //            Assert.IsTrue(  curGenerator?.PrevState?.GetOpenedForGeneration().Count == 0);

        //        // --- resolve deadlock
        //        if (curGenerator.CurrentState.Deadlock != null)
        //        {
        //            Debug.Log("Resolving DEADLOCK");
        //            var opened = curGenerator.CurrentState.GetOpenedForGeneration(); 
        //            if (opened.Count == 0)
        //            {
        //                opened = Pointers.PointerStable.TraverseDepthFirstPostOrder().Where(x =>
        //                    x.Data.Topology.EntityType == Entity.EntityType.ChunkRoofPeak &&
        //                    x != curGenerator.CurrentState.Deadlock).ToList();

        //                //opened = curGenerator.CurrentState.Created.Where(x =>
        //                //    x.Data.Topology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkRoofPeak &&
        //                //    x != curGenerator.CurrentState.Deadlock).ToList();

        //                //opened.AddRange(curGenerator.PrevState.Created.Where(x =>
        //                //    x.Data.Topology.ChunkT == Blueprint.Segment.TopologySegment.ChunkType.ChunkRoofPeak &&
        //                //    x != curGenerator.CurrentState.Deadlock));

        //                Debug.Log($"switching trunk to roof {opened}");
        //            }

        //            Assert.IsTrue(opened.Count != 0);
        //            var topMost = opened.OrderBy(x => x.Data.Topology.Geometry.Position.y).Last();
        //            if (topMost.Data.Topology.EntityType == Entity.EntityType.ChunkRoofPeak)
        //            {
        //                topMost.Data.Topology.EntityType = Entity.EntityType.ChunkStd;
        //                topMost.Data.Topology.IsOpenedForGenerator = true;
        //                curGenerator.CurrentState.Created = new Stack<TreeNode<Blueprint.Segment>>();
        //                curGenerator.CurrentState.Created.Push(topMost);

        //                var step = TopologyGeneratorBase.TopGenStep.DoStep(topMost,
        //                    TopologyGeneratorBase.TopGenStep.VisualizationCmd.SegChangeState);

        //                if (TopologyVisualizer?.StepDelay > 0f)
        //                    yield return TopologyVisualizer.Wait();
        //                TopologyVisualizer?.Step(step);
        //            }
        //            Debug.Log($"switching trunk to {topMost}");
        //            TreeNode<Blueprint.Segment>.SwitchTrunk(topMost);
        //        }


        //        //if(prevGenerator!= null)
        //        //    Pointers.MoveGenerator(prevGenerator.CurrentState.Created.FirstOrDefault(
        //        //        x => (x.BranchLevel == 0 && x.Data.Topology.ChunkT==Blueprint.Segment.TopologySegment.ChunkType.ChunkStd
        //        //              && x.Data.Topology.IsOpenedForGenerator == false)
        //        //    )); // just any of new generated segment belonged to trunk and that is stable (will not be recreated or deleted by next TopGen)
        //        Pointers.PointerStable = GetStableNode(curGenerator.CurrentState.GetOpenedForGeneration());
        //        VisualBuilder?.Build(Pointers.PointerStable);

        //        //if (Pointers.DistanceYFactorProgress2Generator() > Pointers.MaxDistanceProgressToGenerator)
        //        //    yield return new WaitUntil(IsNeedToGenerateMore);

        //        // ----- Change to next in chain TopGen
        //        prevGenerator = curGenerator;
        //        _generatorsChooser.Step();
        //        ++generatorChainCounter;

        //    } while (_generatorsChooser.GetCurrent() != null); // end generators chain

        //    // ----- finalization of the tower
        //    Debug.Log("Finalizing tower");
        //    foreach (var cmd in curGenerator.Finalize(seed, curGenerator.CurrentState)) // finalizing with the global seed 
        //    {
        //        if (TopologyVisualizer?.StepDelay > 0f)
        //            yield return TopologyVisualizer.Wait();
        //        TopologyVisualizer?.Step(cmd);
        //        //VisualBuilder?.Step(cmd);
        //        yield return null;
        //    }
        //    VisualBuilder?.Build(null);

        //    yield return null;
        //    State = GeneratorState.Done;
        //}

        private TreeNode<Blueprint.Segment> GetStableNode(List<TreeNode<Blueprint.Segment>> opened)
        {
            TreeNode<Blueprint.Segment> MoveDownToFirstClosedTrunk(TreeNode<Blueprint.Segment> node)
            {
                var p = node;
                while (p != null)
                {
                    if (p.Data.Topology.IsOpenedForGenerator == false && p.BranchLevel == 0)
                        return p;
                    p = p.Parent;
                }
                return null;
            }

            var closed = new List<TreeNode<Blueprint.Segment>>();
            foreach (var treeNode in opened)
                closed.Add(MoveDownToFirstClosedTrunk(treeNode));

            var lowest = closed.OrderBy(x => x.Level).First();
            return lowest;
        }

        //private bool IsNeedToGenerateMore()
        //{
        //    return Pointers.DistanceYFactorProgress2Generator() < MaxDistanceProgressToGenerator;
        //}


    }
}

