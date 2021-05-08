using System;
using System.Collections.Generic;
using System.Linq;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    // ----- Traits:
    // * Only 1 iteration for the trunk
    // * Spawn islands on iteration 0 only

    // ----- Generating algorithm:
    // * if iteration == 0
    //   * generate trunk 
    //   * try to start islands based on chance from cfg for each side
    //      * based on chance end it up with opened segment or not
    //   * end generation for iteration 0  
    // * for each opened branch segments from prev iteration
    //   * based on chance from cfg
    //      * grow it up
    //      * based on chance keep it opened for the next generator or close.
    //        The chance is reversely proportional to iteration

    public class GeneratorLineUp : GeneratorBase
    {
        public GeneratorLineUp(ConfigLineUp cfg)
            : base(cfg)
        {
        }

        public override void Generate(GeneratorProcessor.State state, int iteration)
        {
            var opened = state.OpenedSegments.OrderBy(x => x.BranchLevel).ToList();
            var lineUpCfg = (ConfigLineUp) Config;

            Assert.IsTrue(opened[0].BranchLevel == 0);
            foreach (var openedNode in opened)
            {
                var architect = new SegmentArchitect(
                    _rndTopology.ValueInt(), _rndVisual.ValueInt(), _rndContent.ValueInt(),
                    state.Pointers.PointerGeneratorStable, lineUpCfg,
                    TopologyType.ChunkStd, TopologyType.ChunkStd, TopologyType.ChunkStd
                );

                var success = architect.MakeProjects( openedNode, lineUpCfg.TrunkSegmentsCount, Vector3.zero, Vector3.up);
                Assert.IsTrue(success);

                var project = architect.GetProject(_rndTopology.Range(0, architect.GetProjectVariantsNumber()), out var lastSeg);

                state.Blueprint.AddSubtree(openedNode, Vector3.up, project);
                state.OpenedSegments.Remove(openedNode);
                state.OpenedSegments.Add(lastSeg);
            }
        }

        //public override IEnumerable<TopGenStep> GenerateTower()
        //{
        //    throw new NotImplementedException();
        //    ////if (Iteration == 0)
        //    //{
        //    //    foreach (var topGenStep in GenerateTrunk()) 
        //    //        yield return topGenStep;

        //    //    // start island
        //    //    if (State.Created.Count <= 3)
        //    //        yield return null;
        //    //}
        //}

        //private IEnumerable<TopGenStep> GenerateTrunk()
        //{
        //    throw new NotImplementedException();
        //    //var segBuilder = new SegmentBuilder(this, _rnd.ValueInt());

        //    //segBuilder.Project(
        //    //    State.GetOpenedTrunkNode(),
        //    //    Config.TrunkSegmentsCount,
        //    //    Vector3.up,
        //    //    Vector3.zero,
        //    //    Config.GetPlacementConfig(Entity.EntityType.ChunkStd),
        //    //    Config.GetPlacementConfig(Entity.EntityType.ChunkStd),
        //    //    Config.GetPlacementConfig(Entity.EntityType.ChunkStd)
        //    //);

        //    //// deadlock
        //    //if (!segBuilder.IsProjectInRange())
        //    //{
        //    //    //State.IsStillGeneratingTrunk = false;
        //    //    State.TrunkDeadlock = State.GetOpenedTrunkNode();
        //    //    yield return null;
        //    //}

        //    //segBuilder.ApplyProjectRnd();

        //    //// build trunk
        //    //foreach (var segment in segBuilder.Build())
        //    //    yield return TopGenStep.DoStep(segment, TopGenStep.Cmd.SegSpawn);
        //    ////State.IsStillGeneratingTrunk = false;
        //}
        public override bool Done()
        {
            return true;
        }
    }
}