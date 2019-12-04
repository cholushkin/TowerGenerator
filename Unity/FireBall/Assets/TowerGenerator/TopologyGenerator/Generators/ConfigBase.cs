using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    public abstract class ConfigBase : MonoBehaviour
    {
        public abstract GeneratorBase CreateGenerator(
            long seed,
            TreeNode<Blueprint.Segment> startFrom,
            TopologyGeneratorsManifoldBase manifold
        );

        [Serializable]
        public class PlacementConfig
        {
            public enum SizeStrategy
            {
                ChunkRndSize,
                ChunkMaxSize,
                ChunkMinSize
            }
            public Entity.EntityType ChunkEntityType;
            public SizeStrategy ChunkSizeStrategy;
            public bool IgnoreChunkSizeRestrictions;
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

        public Range SegmentsBudget; // like a real a budget could be overused (just approximation of height size in segments)
        public List<PlacementConfig> PlacementConfigs;
        public AllowedDirectionsChances AllowedDirections;
        private readonly float[] _dirChances = new float[6];

        private static readonly Vector3[] _directions =
        {
            Vector3.left, Vector3.right, Vector3.up,
            Vector3.down, Vector3.forward, Vector3.back,
        };


        public PlacementConfig GetPlacementConfig(Entity.EntityType entType)
        {
            var cfg = PlacementConfigs.FirstOrDefault(x => x.ChunkEntityType == entType) ?? 
                      PlacementConfigs.FirstOrDefault(x => x.ChunkEntityType == Entity.EntityType.Undefined); // default
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
