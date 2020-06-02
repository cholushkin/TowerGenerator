using System;
using System.Collections.Generic;
using System.Linq;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    public abstract class GeneratorConfigBase : MonoBehaviour
    {
        public abstract GeneratorBase CreateGenerator(long seed);

        [Serializable]
        public class PlacementConfig
        {
            //public enum SizeStrategy
            //{
            //    ChunkRndSize,
            //    ChunkMaxSize,
            //    ChunkMinSize
            //}
            public TopologyType TopologyType; // todo: to dictionary key
            //public SizeStrategy ChunkSizeStrategy;
            //public bool IgnoreChunkSizeRestrictions; // todo: use zero ranges instead
            public Range SegmentsSizeBreadth;
            public Range SegmentsSizeHeight;
            // todo: tags
            // todo: purpose of segements
        }

        [Serializable]
        public class ChunkSpecificPlacement
        {
            public PlacementConfig PlacementConfig;
        }

        [Serializable]
        public class AllowedDirectionsChances
        {
            [Range(0f, 1f)] public float Left;
            [Range(0f, 1f)] public float Right;
            [Range(0f, 1f)] public float Up;
            [Range(0f, 1f)] public float Down;
            [Range(0f, 1f)] public float Forward;
            [Range(0f, 1f)] public float Back;
        }

        public Range TrunkSegmentsCount; 
        public List<PlacementConfig> PlacementConfigs; // todo: to serizalizable dictionary

        [Tooltip("Placement cfg of initial chunk that current config recommend to use (accordingly to other chunks)")]
        public PlacementConfig EstablishPlacement;

        public AllowedDirectionsChances AllowedDirections;

        public long SeedTopology;
        public long SeedVisual;
        public long SeedContent;

        public bool ResetOnProcessorEnter;

        private long _initialSeedTopology;
        private long _initialSeedVisual;
        private long _initialSeedContent;

        private readonly float[] _dirChances = new float[6];

        private static readonly Vector3[] _directions =
        {
            Vector3.left, Vector3.right, Vector3.up,
            Vector3.down, Vector3.forward, Vector3.back,
        };


        // replace all -1 in configs and save initial values
        public void Init(long seed)
        {   
            var rnd = new RandomHelper(seed);
            if (SeedContent == -1)
                SeedContent = rnd.ValueInt();

            if (SeedTopology == -1)
                SeedTopology = rnd.ValueInt();

            if (SeedVisual == -1)
                SeedVisual = rnd.ValueInt();

            _initialSeedTopology = SeedContent;
            _initialSeedVisual = SeedVisual;
            _initialSeedContent = SeedContent;
        }

        public void OnProcessorEnter()
        {
            if (ResetOnProcessorEnter)
            {
                ResetSeeds();
            }
        }

        public void ResetSeeds()
        {
            SeedContent = _initialSeedContent;
            SeedVisual = _initialSeedVisual;
            SeedTopology = _initialSeedTopology;
        }

        public PlacementConfig GetPlacementConfig(TopologyType topType)
        {
            var cfg = PlacementConfigs.FirstOrDefault(x => x.TopologyType.HasFlag(topType)) ?? 
                      PlacementConfigs.FirstOrDefault(x => x.TopologyType == TopologyType.Undefined); // default
            return cfg;
        }

        public Vector3 GetRndPropagationDir(ref RandomHelper rnd)
        {
            _dirChances[0] = AllowedDirections.Left;
            _dirChances[1] = AllowedDirections.Right;
            _dirChances[2] = AllowedDirections.Up;
            _dirChances[3] = AllowedDirections.Down;
            _dirChances[4] = AllowedDirections.Forward;
            _dirChances[5] = AllowedDirections.Back;

            var rndDirIndex = rnd.SpawnEvent(_dirChances);
            return _directions[rndDirIndex];
        }

        //public Vector3 GetRndSegSize(ref RandomHelper rnd, Entity.EntityType entType)
        //{
        //    var placementCfg = ChunkSpecific.FirstOrDefault(e => e.ApplyTo == entType)?.Placement ?? DefaultPlacement;
        //    var breadth = rnd.FromRange(placementCfg.SegmentsSizeBreadth);
        //    var height = rnd.FromRange(placementCfg.SegmentsSizeHeight);
        //    return new Vector3(breadth, height, breadth);
        //}

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
