using System.Collections.Generic;
using GameLib.DataStructures;
using UnityEngine;

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
        public GeneratorLineUp(long seed, TreeNode<Blueprint.Segment> trunkNode, GeneratorConfigBase cfg, TopologyGeneratorsManifoldBase manifold) 
            : base(seed, trunkNode, cfg, manifold)
        {
        }

        public override IEnumerable<TopGenStep> GenerateTower()
        {
            if (Iteration == 0)
            {
                foreach (var topGenStep in GenerateTrunk()) 
                    yield return topGenStep;
                
                // start island
                if (State.Created.Count <= 3)
                    yield return null;
            }
        }

        private IEnumerable<TopGenStep> GenerateTrunk()
        {
            var segBuilder = new SegmentBuilder(this, _rnd.ValueInt());

            segBuilder.Project(
                State.GetOpenedTrunkNode(),
                Config.SegmentsBudget,
                Vector3.up,
                Vector3.zero,
                Config.GetPlacementConfig(Entity.EntityType.ChunkStd),
                Config.GetPlacementConfig(Entity.EntityType.ChunkStd),
                Config.GetPlacementConfig(Entity.EntityType.ChunkStd)
            );

            // deadlock
            if (!segBuilder.IsProjectInRange())
            {
                State.IsStillGeneratingTrunk = false;
                State.TrunkDeadlock = State.GetOpenedTrunkNode();
                yield return null;
            }

            segBuilder.ApplyProjectRnd();

            // build trunk
            foreach (var segment in segBuilder.Build())
                yield return TopGenStep.DoStep(segment, TopGenStep.Cmd.SegSpawn);
            State.IsStillGeneratingTrunk = false;
        }
    }
}