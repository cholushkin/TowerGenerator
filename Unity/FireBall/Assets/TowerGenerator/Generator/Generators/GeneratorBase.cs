using System.Collections;
using System.Collections.Generic;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    public abstract class GeneratorBase
    {
        public GeneratorConfigBase Config { get; private set; }
        protected RandomHelper _rndTopology;
        protected RandomHelper _rndVisual;
        protected RandomHelper _rndContent;

        public GeneratorBase(GeneratorConfigBase cfg)
        {
            _rndTopology = new RandomHelper(cfg.SeedTopology);
            _rndVisual = new RandomHelper(cfg.SeedVisual);
            _rndContent = new RandomHelper(cfg.SeedContent);

            Config = cfg;
        }

        public abstract void Generate(GeneratorProcessor.State state, int iteration);

        public abstract bool Done();
    }


}

