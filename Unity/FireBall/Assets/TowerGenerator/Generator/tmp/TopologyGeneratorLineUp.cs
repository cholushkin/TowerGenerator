//using System;
//using System.Collections.Generic;
//using System.Linq;
//using GameLib.DataStructures;
//using GameLib.Random;
//using UnityEngine;
//using UnityEngine.Assertions;

//namespace TowerGenerator
//{
//    // ----- Build sequence:
//    // * if there is no parent build island
//    // * build trunk up
//    // * try to generate islands based on chance from cfg for each side
//    //   * based on chance end it up with opened segment or not
//    // * for each opened segments from prev state
//    //   * based on chance from cfg
//    //      * grow it up
//    //        * base on chance keep it opened for the next generator
//    //      * close 

//    public class TopologyGeneratorLineUp : TopologyGeneratorBase
//    {
//        [Serializable]
//        public class TopologyGeneratorConfigLineUp : TopologyGeneratorConfigBase
//        {
//            public Range IslandHeight;
//            public Range IslandDistanceFromTrunk;

//            [Range(0f, 1f)]
//            public float PropagateIslandChance; // used for creating openings on branches(use case 1) and for continue openings from prev generator (2)
//        }

//        public TopologyGeneratorConfigLineUp Config;

//        public override IEnumerable<TopGenStep> Generate(uint seed, GeneratorState prevState)
//        {
//            PrevState = prevState;
//            ClearCurrentState();
//            _rnd.SetCurrentSeed(seed);

//            // get opened
//            var opened = prevState?.GetOpenedForGeneration();
//            if (opened == null)
//            {
//                opened = new List<TreeNode<Blueprint.Segment>>((int)Config.TrunkSegmentsCount.To);
//                var island = CreateOriginIsland(Config);
//                opened.Add(island);
//                yield return TopGenStep.DoStep(CurrentState.Created.Peek(), TopGenStep.VisualizationCmd.SegSpawn);
//            }

//            Assert.IsNotNull(opened);
//            Assert.IsTrue(opened.Count > 0);

//            // take all 'OpenedForGeneration' segments from prev creator in 'Created'
//            // close all that is not trunk
//            opened = opened.OrderBy(x => x.BranchLevel).ToList(); // sort: trunk first
//            foreach (var openedNode in opened)
//            {
//                if (openedNode.BranchLevel == 0)
//                {
//                    var trunkSegCount = _rnd.FromRangeIntInclusive(Config.TrunkSegmentsCount);
//                    var trunkSegCountGenerated = 0;

//                    // --- build the trunk
//                    {
//                        //Debug.LogFormat("Generating trunk");
//                        Assert.IsTrue(openedNode.Data.Topology.IsOpenedForGenerator);

//                        var segBuilder = new SegmentBuilder(trunkSegCount, openedNode, Vector3.up, this, Vector3.zero);

//                        foreach (var stepResult in segBuilder.Step())
//                        {
//                            if (stepResult.IsDeadlock)
//                            // --- change parent to peak, close it and register deadlock in generator
//                            {
//                                var deadlockParent = stepResult.Prev.Segment;
//                                Assert.IsNotNull(deadlockParent);
//                                Debug.LogWarning($"DEADLOCK. Unable to find fit size for the TRUNK segment");
//                                Debug.LogWarning($"parent {deadlockParent}, fitSize {stepResult.Size}, config {Config}");
//                                CurrentState.Deadlock = deadlockParent;

//                                Assert.IsTrue(deadlockParent.Data.Topology.EntityType == Entity.EntityType.ChunkStd);
//                                deadlockParent.Data.Topology.EntityType = Entity.EntityType.ChunkRoofPeak;
//                                deadlockParent.Data.Topology.IsOpenedForGenerator = false;
//                                yield return TopGenStep.DoStep(deadlockParent, TopGenStep.VisualizationCmd.SegChangeState);
//                                break;
//                            }

//                            if (stepResult.IsFirstOne)
//                            // --- close parent
//                            {
//                                Assert.IsTrue(stepResult.Prev.Segment.Data.Topology.IsOpenedForGenerator);
//                                stepResult.Prev.Segment.Data.Topology.IsOpenedForGenerator = false;
//                                yield return TopGenStep.DoStep(stepResult.Prev.Segment, TopGenStep.VisualizationCmd.SegChangeState);
//                            }

//                            // real build
//                            stepResult.BuildSegment(Entity.EntityType.ChunkStd, stepResult.IsLastOne);
//                            ++trunkSegCountGenerated;
//                            yield return TopGenStep.DoStep(stepResult.Segment, TopGenStep.VisualizationCmd.SegSpawn);
//                        }
//                    }
                  
//                    // --- generate islands
//                    if (trunkSegCountGenerated > 3)
//                    {
//                        var parentLeft = CurrentState.Created.ElementAt(1 + _rnd.Range(0, trunkSegCountGenerated - 1));
//                        var parentRight = CurrentState.Created.ElementAt(1 + _rnd.Range(0, trunkSegCountGenerated - 1));
//                        var parentForward = CurrentState.Created.ElementAt(1 + _rnd.Range(0, trunkSegCountGenerated - 1));
//                        var parentBack = CurrentState.Created.ElementAt(1 + _rnd.Range(0, trunkSegCountGenerated - 1));

//                        if (_rnd.TrySpawnEvent(Config.AllowedDirections.Left))
//                        {
//                            foreach (var step in GenerateSideIsland(parentLeft, Vector3.left,
//                                Vector3.left * _rnd.FromRange(Config.IslandDistanceFromTrunk)))
//                                yield return step;
//                        }

//                        if (_rnd.TrySpawnEvent(Config.AllowedDirections.Right))
//                        {
//                            foreach (var step in GenerateSideIsland(parentRight, Vector3.right,
//                                Vector3.right * _rnd.FromRange(Config.IslandDistanceFromTrunk)))
//                                yield return step;
//                        }

//                        if (_rnd.TrySpawnEvent(Config.AllowedDirections.Forward))
//                        {
//                            foreach (var step in GenerateSideIsland(parentForward, Vector3.forward,
//                                Vector3.forward * _rnd.FromRange(Config.IslandDistanceFromTrunk)))
//                                yield return step;
//                        }

//                        if (_rnd.TrySpawnEvent(Config.AllowedDirections.Back))
//                        {
//                            foreach (var step in GenerateSideIsland(parentBack, Vector3.back,
//                                Vector3.back * _rnd.FromRange(Config.IslandDistanceFromTrunk)))
//                                yield return step;
//                        }
//                    }
//                }
//                else // end up segments from prev generator state
//                {
//                    if (_rnd.TrySpawnEvent(Config.PropagateIslandChance)) // continue branches from previous generator
//                    {
//                        Debug.LogFormat("Generating branch from previous state");
//                        Assert.IsTrue(openedNode.Data.Topology.IsOpenedForGenerator);

//                        var segCount = _rnd.FromRangeIntInclusive(Config.IslandHeight);
//                        var segBuilder = new SegmentBuilder(segCount, openedNode, Vector3.up, this, Vector3.zero);

//                        foreach (var stepResult in segBuilder.Step())
//                        {
//                            if (stepResult.IsDeadlock)
//                            // --- change parent to peak, close it
//                            {
//                                var deadlockParent = stepResult.Prev.Segment;
//                                Assert.IsNotNull(deadlockParent);
//                                Debug.LogWarning("Unable to find fit size for the branch segment. Closing branch");
//                                Debug.LogWarning($"parent {deadlockParent}, fitSize {stepResult.Size}, config {Config}");
//                                CurrentState.Deadlock = deadlockParent;

//                                Assert.IsTrue(deadlockParent.Data.Topology.EntityType == Entity.EntityType.ChunkStd);
//                                deadlockParent.Data.Topology.EntityType = Entity.EntityType.ChunkRoofPeak;
//                                deadlockParent.Data.Topology.IsOpenedForGenerator = false;
//                                yield return TopGenStep.DoStep(deadlockParent, TopGenStep.VisualizationCmd.SegChangeState);
//                                break;
//                            }

//                            if (stepResult.IsFirstOne)
//                            // --- close first segment parent
//                            {
//                                Assert.IsTrue(stepResult.Prev.Segment.Data.Topology.IsOpenedForGenerator);
//                                stepResult.Prev.Segment.Data.Topology.IsOpenedForGenerator = false;
//                                yield return TopGenStep.DoStep(stepResult.Prev.Segment, TopGenStep.VisualizationCmd.SegChangeState);
//                            }

//                            // real build
//                            stepResult.BuildSegment(Entity.EntityType.ChunkStd, false);

//                            if (stepResult.IsLastOne)
//                            {
//                                if (_rnd.TrySpawnEvent(Config.PropagateIslandChance)) // propagate further ?
//                                    stepResult.Segment.Data.Topology.IsOpenedForGenerator = true;
//                                else
//                                    stepResult.Segment.Data.Topology.EntityType = Entity.EntityType.ChunkRoofPeak;
//                            }

//                            yield return TopGenStep.DoStep(stepResult.Segment, TopGenStep.VisualizationCmd.SegSpawn);
//                        }
//                    }
//                    else 
//                    // end up with peak and close
//                    {
//                        openedNode.Data.Topology.EntityType = Entity.EntityType.ChunkRoofPeak;
//                        openedNode.Data.Topology.IsOpenedForGenerator = false;
//                        yield return TopGenStep.DoStep(openedNode, TopGenStep.VisualizationCmd.SegChangeState);
//                    }
//                }
//            }
//        }

//        public override TopologyGeneratorConfigBase GetConfig()
//        {
//            return Config;
//        }

//        private IEnumerable<TopGenStep> GenerateSideIsland(TreeNode<Blueprint.Segment> from, Vector3 side, Vector3 offsetFromTrunk)
//        {
//            Assert.IsTrue(Config.IslandHeight.From >= 3);

//            var branchSegCount = _rnd.FromRangeIntInclusive(Config.IslandHeight);
//            var builder = new SegmentBuilder(branchSegCount, from, side, this, offsetFromTrunk);
//            Assert.IsTrue(branchSegCount >= 3);

//            var cnt = 0;
//            foreach (var step in builder.Step())
//            {
//                if (!step.IsDeadlock)
//                    ++cnt;
//                builder.CurrentDirection = Vector3.up; // after first step move up
//            }

//            if (cnt >= 3)
//            {
//                foreach (var step in builder.Steps())
//                {
//                    step.BuildSegment();
//                    if (step.IsFirstOne)
//                    {
//                        step.Segment.Data.Topology.Connection = Vector3.zero;
//                        step.Segment.Data.Topology.EntityType = Entity.EntityType.ChunkIslandAndBasement;
//                    }

//                    if (step.IsLastOne)
//                    {
//                        if (_rnd.TrySpawnEvent(Config.PropagateIslandChance)) // opened and std
//                            step.Segment.Data.Topology.IsOpenedForGenerator = true;
//                        else
//                            step.Segment.Data.Topology.EntityType = Entity.EntityType.ChunkRoofPeak;
//                    }
//                    yield return TopGenStep.DoStep(step.Segment, TopGenStep.VisualizationCmd.SegSpawn);
//                }
//            }
//        }
//    }
//}