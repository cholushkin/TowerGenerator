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
        protected RandomHelper _rnd;

        public GeneratorBase(long seed, GeneratorConfigBase cfg)
        {
            _rnd = new RandomHelper(seed);
            Config = cfg;
        }

        public abstract void Generate(GeneratorProcessor.State state, int iteration);

        public abstract bool Done();
    }


}

