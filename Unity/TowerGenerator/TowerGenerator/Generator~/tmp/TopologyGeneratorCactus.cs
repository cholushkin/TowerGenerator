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
//    // * if there is no parent build island (initial)
//    // * build trunk up (using cfg.TrunkSegmentsCount)
//    // * try to generate branches based on chance from cfg for each side
//    //   * based on chance end it up with opened segment or not
//    //   * by cfg.BranchLev1GrowingDownChance build branch down on 1-2 segments if possible
//    // * based on chance (50% of prev level branches chance) spawn subbranches
//    //   * also conditions for spawn subbranches
//    //     * BranchSegmentsBudget * 0.5 of prev level but not less than 5 (if less don't build branches)
//    //     * BranchSegmentsSize *0.5 of prev level but not less than minimum of BranchSegmentsSize.From (if less then don't generate further)
//    // * for each opened segments from prev state treat it as branch of cactus of apropriate level and finish it with chance for that level (or close) // todo:
//    // * if DiversionChance hit
//    //   * if trunk is far from center.
//    //     * find big enough opened segment closest to central axis and Switch trunk to that branch
//    //   * if trunk is too close to center
//    //     * find big enough opened segment that is most remote from to the central axis and Switch trunk to that branch

//    public class TopologyGeneratorCactus : TopologyGeneratorBase
//    {
//        [Serializable]
//        public class TopologyGeneratorConfigCactus : TopologyGeneratorConfigBase
//        {
//            [Range(0f, 1f)] public float BranchPropagateChance; // шанс доращивать бранчи от предыдущего генераторо или продлевать бранчи текущего генераторв

//            [Range(0f, 1f)] public float BranchLev1GrowingDownChance; // шанс растить бранч вниз for level 1 branch only

//            [Range(0f, 1f)]
//            public float DiversionChance; // шанс отклонится от центра ( вернуться или отойти от центральной оси)

//            public bool ClampSegSizeByParentForTrunk;
//            public bool ClampSegSizeByParentForBranch;

//            public Range BranchSegmentsBudget;
//            public Range BranchSegmentsSize;

//            public int maxBranchLevel; // 0 - trunk
//        }

//        public TopologyGeneratorConfigCactus Config;

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
//            // and continue them or close them starting from trunk
//            opened = opened.OrderBy(x => x.BranchLevel).ToList();
//            foreach (var openedNode in opened)
//            {
//                if (openedNode.BranchLevel == 0) // for the trunk
//                {
//                    var trunkSegCount = _rnd.FromRangeIntInclusive(Config.TrunkSegmentsCount);
//                    Assert.IsTrue(openedNode.Data.Topology.IsOpenedForGenerator);
//                    var segBuilder = new SegmentBuilder(trunkSegCount, openedNode, Vector3.up, this, Vector3.zero);

//                    // --- build the trunk
//                    {
//                        //Debug.LogFormat("Generating trunk");

//                        foreach (var stepResult in segBuilder.Step())
//                        {
//                            if (stepResult.IsDeadlock)
//                            // --- change parent to peak, close it and register deadlock in generator
//                            {
//                                var deadlockParent = stepResult.Prev.Segment;
//                                Assert.IsNotNull(deadlockParent);
//                                Debug.LogWarning("DEADLOCK. Unable to find fit size for the TRUNK segment");
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
//                                Assert.IsTrue(stepResult.Prev.Segment.Data.Topology.EntityType != Entity.EntityType.ChunkRoofPeak);
//                                stepResult.Prev.Segment.Data.Topology.IsOpenedForGenerator = false;
//                                yield return TopGenStep.DoStep(stepResult.Prev.Segment, TopGenStep.VisualizationCmd.SegChangeState);
//                            }

//                            // real build
//                            stepResult.BuildSegment(Entity.EntityType.ChunkStd, stepResult.IsLastOne);
//                            yield return TopGenStep.DoStep(stepResult.Segment, TopGenStep.VisualizationCmd.SegSpawn);
//                        }
//                    }

//                    // generate new branches
//                    {
//                        Stack<SegmentBuilder> builders = new Stack<SegmentBuilder>();
//                        builders.Push(segBuilder);
//                        var parentBranchLevel = 0;

//                        while (builders.Count > 0) // process current branch level
//                        {
//                            if (parentBranchLevel >= Config.maxBranchLevel)
//                                break;
//                            var thisLevelBuilders = builders.ToArray();
//                            builders.Clear();

//                            Debug.Log($"Building branches of level {parentBranchLevel + 1}");
//                            foreach (var segmentBuilder in thisLevelBuilders) // process each branch
//                            {
//                                var created = segmentBuilder.Steps().Where(x => x.IsDeadlock == false).ToArray();
//                                if (created.Length < 4)
//                                    continue;
//                                Assert.IsTrue(created[0].Segment.BranchLevel == parentBranchLevel);

//                                // get roots for each branch
//                                var parentLeft = created.ElementAt(_rnd.Range(1, created.Length - 2)).Segment;
//                                var parentRight = created.ElementAt(_rnd.Range(1, created.Length - 2)).Segment;
//                                var parentForward = created.ElementAt(_rnd.Range(1, created.Length - 2)).Segment;
//                                var parentBack = created.ElementAt(_rnd.Range(1, created.Length - 2)).Segment;

//                                if (_rnd.TrySpawnEvent(Config.AllowedDirections.Left) && _rnd.TrySpawnEvent(GetChanceToPropagateBranch(parentBranchLevel)))
//                                {
//                                    Assert.IsTrue(Config.BranchSegmentsBudget.From >= 6);
//                                    var branchSegCount = _rnd.FromRangeIntInclusive(Config.BranchSegmentsBudget);
//                                    var builder = new SegmentBuilder(branchSegCount, parentLeft, Vector3.left, this, Vector3.zero);
//                                    builder.SetSegmentSize(Config.BranchSegmentsSize); // todo: based on branch level
//                                    Assert.IsTrue(branchSegCount >= 6);
//                                    foreach (var step in GenerateSideBranch(builder, parentBranchLevel))
//                                        yield return step;
//                                    if (builder.CreatedCount > 0)
//                                        builders.Push(builder); // push only if current builder created anything
//                                }
//                                if (_rnd.TrySpawnEvent(Config.AllowedDirections.Right) && _rnd.TrySpawnEvent(GetChanceToPropagateBranch(parentBranchLevel)))
//                                {
//                                    Assert.IsTrue(Config.BranchSegmentsBudget.From >= 6);
//                                    var branchSegCount = _rnd.FromRangeIntInclusive(Config.BranchSegmentsBudget);
//                                    var builder = new SegmentBuilder(branchSegCount, parentRight, Vector3.right, this, Vector3.zero);
//                                    builder.SetSegmentSize(Config.BranchSegmentsSize); // todo: based on branch level
//                                    Assert.IsTrue(branchSegCount >= 6);
//                                    foreach (var step in GenerateSideBranch(builder, parentBranchLevel))
//                                        yield return step;
//                                    if (builder.CreatedCount > 0)
//                                        builders.Push(builder); // push only if current builder created anything
//                                }
//                                if (_rnd.TrySpawnEvent(Config.AllowedDirections.Forward) && _rnd.TrySpawnEvent(GetChanceToPropagateBranch(parentBranchLevel)))
//                                {
//                                    Assert.IsTrue(Config.BranchSegmentsBudget.From >= 6);
//                                    var branchSegCount = _rnd.FromRangeIntInclusive(Config.BranchSegmentsBudget);
//                                    var builder = new SegmentBuilder(branchSegCount, parentForward, Vector3.forward, this, Vector3.zero);
//                                    builder.SetSegmentSize(Config.BranchSegmentsSize); // todo: based on branch level
//                                    Assert.IsTrue(branchSegCount >= 6);
//                                    foreach (var step in GenerateSideBranch(builder, parentBranchLevel))
//                                        yield return step;
//                                    if (builder.CreatedCount > 0)
//                                        builders.Push(builder); // push only if current builder created anything
//                                }
//                                if (_rnd.TrySpawnEvent(Config.AllowedDirections.Back) && _rnd.TrySpawnEvent(GetChanceToPropagateBranch(parentBranchLevel)))
//                                {
//                                    Assert.IsTrue(Config.BranchSegmentsBudget.From >= 6);
//                                    var branchSegCount = _rnd.FromRangeIntInclusive(Config.BranchSegmentsBudget);
//                                    var builder = new SegmentBuilder(branchSegCount, parentBack, Vector3.back, this, Vector3.zero);
//                                    builder.SetSegmentSize(Config.BranchSegmentsSize); // todo: based on branch level
//                                    Assert.IsTrue(branchSegCount >= 6);
//                                    foreach (var step in GenerateSideBranch(builder, parentBranchLevel))
//                                        yield return step;
//                                    if (builder.CreatedCount > 0)
//                                        builders.Push(builder); // push only if current builder created anything
//                                }
//                            }
//                            parentBranchLevel++;
//                        }
//                    }
//                }
//                else
//                // end up with peak and close
//                {
//                    openedNode.Data.Topology.EntityType = Entity.EntityType.ChunkRoofPeak;
//                    openedNode.Data.Topology.IsOpenedForGenerator = false;
//                    yield return TopGenStep.DoStep(openedNode, TopGenStep.VisualizationCmd.SegChangeState);
//                }
//            }

//            if (_rnd.TrySpawnEvent(Config.DiversionChance) && CurrentState.Deadlock == null)
//            {
//                const float distanceFromCenterMax = 10f;
//                var trunk = opened.First();
//                Assert.IsNotNull(trunk);
//                Assert.IsTrue(trunk.BranchLevel == 0);

//                var openedForGeneration = CurrentState.GetOpenedForGeneration();
//                openedForGeneration = openedForGeneration.OrderBy(DistanceToCentralAxis).ToList();
//                Debug.Log("Switching trunk to");
//                TreeNode<Blueprint.Segment>.SwitchTrunk(DistanceToCentralAxis(trunk) > distanceFromCenterMax
//                    ? openedForGeneration.First()
//                    : openedForGeneration.Last());
//            }
//        }

//        public override TopologyGeneratorConfigBase GetConfig()
//        {
//            return Config;
//        }

//        float DistanceToCentralAxis(TreeNode<Blueprint.Segment> trunkNode)
//        {
//            Assert.IsNotNull(trunkNode);
//            var planePos = trunkNode.Data.Topology.Geometry.Position;
//            planePos.y = 0f;
//            return planePos.magnitude;
//        }

//        private IEnumerable<TopGenStep> GenerateSideBranch(SegmentBuilder builder, int level)
//        {
//            var cnt = 0;
//            var branchVerticalPartCount = _rnd.Range(2, 4); // 2 or 3
//            Assert.IsTrue(branchVerticalPartCount == 2 || branchVerticalPartCount == 3);

//            foreach (var step in builder.Step())
//            {
//                if (!step.IsDeadlock)
//                    ++cnt;
//                if (--branchVerticalPartCount == 0)
//                    builder.CurrentDirection = Vector3.up; // after branchVerticalPartCount steps move up
//            }

//            if (cnt >= 6)
//            {
//                foreach (var step in builder.Steps())
//                {
//                    step.BuildSegment(); // std and closed by default
//                    if (step.IsLastOne)
//                    {
//                        if (_rnd.TrySpawnEvent(GetChanceToPropagateBranch(level + 1))) // opened and std
//                            step.Segment.Data.Topology.IsOpenedForGenerator = true;
//                        else
//                            step.Segment.Data.Topology.EntityType = Entity.EntityType.ChunkRoofPeak;
//                    }
//                    yield return TopGenStep.DoStep(step.Segment, TopGenStep.VisualizationCmd.SegSpawn);
//                }
//            }
//        }

//        private float GetChanceToPropagateBranch(int branchLevel)
//        {
//            return Config.BranchPropagateChance * Mathf.Pow(0.75f, branchLevel);
//        }

//    } // TopologyGeneratorCactus
//} // namepace