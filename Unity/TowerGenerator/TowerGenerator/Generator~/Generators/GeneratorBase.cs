using System.Collections;
using System.Collections.Generic;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public abstract class GeneratorBase
    {
        public GeneratorConfigBase Config { get; private set; }
        protected IPseudoRandomNumberGenerator _rndTopology;
        protected IPseudoRandomNumberGenerator _rndVisual;
        protected IPseudoRandomNumberGenerator _rndContent;

        public GeneratorBase(GeneratorConfigBase cfg)
        {
            _rndTopology = RandomHelper.CreateRandomNumberGenerator(cfg.SeedTopology);
            _rndVisual = RandomHelper.CreateRandomNumberGenerator(cfg.SeedVisual);
            _rndContent = RandomHelper.CreateRandomNumberGenerator(cfg.SeedContent);

            Config = cfg;
        }

        public abstract void Generate(GeneratorProcessor.State state, int iteration);

        public virtual void Finalize(GeneratorProcessor.State state)
        {
            foreach (var stateOpenedSegment in state.OpenedSegments)
            {
                var architect = new SegmentArchitect(
                    Config.SeedTopology, Config.SeedContent, Config.SeedVisual,
                    state.Pointers.PointerGeneratorStable, Config,
                    TopologyType.ChunkPeak, TopologyType.Undefined, TopologyType.Undefined);

                // try to get specific finalization first for this config
                var finalizationPlacementCfg = Config.GetPlacementConfig(TopologyType.ChunkPeak, SpecificsStringConstants.Finalization);
                if (finalizationPlacementCfg == null)
                {
                    finalizationPlacementCfg = Config.GetPlacementConfig(TopologyType.ChunkPeak);
                    Assert.IsNotNull(finalizationPlacementCfg);
                }
                architect.PlacementConfigProvider = (segRelativePos, segIndex) => finalizationPlacementCfg;

                architect.MakeProjects(
                    stateOpenedSegment,
                    Range.One,
                    Vector3.zero, Vector3.up
                );

                var projectsNumber = architect.GetProjectVariantsNumber();
                Assert.IsTrue(projectsNumber == 0 || projectsNumber == 1);

                if (projectsNumber == 0) // couldn't finalize the tower due to collision occured
                {
                    Debug.LogError("couldn't finalize the tower due to collision occured");
                    state.DeadlockSegments.Add(stateOpenedSegment);
                }
                else
                {
                    Assert.IsTrue(architect.GetProjectVariantsNumber() == 1);
                    var project = architect.GetProject(0, out var lastNode);
                    state.Blueprint.AddSubtree(stateOpenedSegment, Vector3.up, project);
                }
            }
            state.OpenedSegments.Clear();
        }

        public abstract bool Done();
    }


}

