using System.Collections;
using System.Collections.Generic;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    public class ConfigLineUp : GeneratorConfigBase
    {
        public Range IslandHeight;
        public Range IslandDistanceFromTrunk;

        [Range(0f, 1f)]
        public float PropagateIslandChance; // used for creating openings on branches(use case 1) and for continue openings from prev generator (2)

        public override GeneratorBase CreateGenerator( long seed )
        {
            return new GeneratorLineUp(seed, this);
        }
    }

}