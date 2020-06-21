using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Plugins.Alg;
using GameLib.Random;
using NCalc;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public static class SpecificsStringConstants
    {
        public const string Establishment = "Establishment";
    }

    public abstract class GeneratorConfigBase : MonoBehaviour
    {
        public abstract GeneratorBase CreateGenerator(long seed);

        [Serializable]
        public class PlacementConfig
        {
            public bool IsStrictSpecifics; // true means that this PlacementConfig is applicable only when it was requested with Expression that fits with Specifics.
            public TagSet Specifics; // example: HugePeeks, VerticalIslandOnly or Establishments
            public MetaProvider.Filter MetaFilter;
            public MetaProvider[] MetaProviders; // if empty then use all of them
            public TopologyType TopologyType => MetaFilter.TopologyType;
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
        public List<PlacementConfig> PlacementConfigs;


        public AllowedDirectionsChances AllowedDirections;

        public long SeedTopology;
        public long SeedVisual;
        public long SeedContent;

        public bool ResetOnProcessorEnter;

        private long _initialSeedTopology;
        private long _initialSeedVisual;
        private long _initialSeedContent;

        private RandomHelper _rnd;

        private readonly float[] _dirChances = new float[6];

        private static readonly Vector3[] _directions =
        {
            Vector3.left, Vector3.right, Vector3.up,
            Vector3.down, Vector3.forward, Vector3.back,
        };


        // replace all -1 in configs and save initial values
        public void Init(long seed, Prototype prototype)
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
            _rnd = new RandomHelper(_initialSeedTopology);

            // init PlacementConfigs list
            Assert.IsFalse(PlacementConfigs == null || PlacementConfigs.Count == 0, $"Generator config has no placement configs assigned {transform.GetDebugName()}");
            foreach (var placementConfig in PlacementConfigs)
            {
                if (placementConfig.MetaProviders == null || placementConfig.MetaProviders.Length == 0)
                {
                    placementConfig.MetaProviders = prototype.MetaProviderManager.MetaProviders;
                }
                Assert.IsNotNull(placementConfig.MetaProviders);
                Assert.IsTrue(placementConfig.MetaProviders!=null && placementConfig.MetaProviders.Length > 0);
            }
        }

        public void OnProcessorEnter()
        {
            if (ResetOnProcessorEnter)
            {
                ResetSeeds();
            }
        }

        public virtual void ResetSeeds()
        {
            SeedContent = _initialSeedContent;
            SeedVisual = _initialSeedVisual;
            SeedTopology = _initialSeedTopology;
            _rnd = new RandomHelper(_initialSeedTopology);
        }

        public PlacementConfig GetPlacementConfig(TopologyType topologyType, string specificationExpression = null)
        {
            // get all placement configs of such topologyType
            var placementConfigs = PlacementConfigs.Where(x => x.TopologyType.HasFlag(topologyType)).ToArray();
            if (!placementConfigs.Any())
                return null;

            // get random with specification
            if (!string.IsNullOrEmpty(specificationExpression))
            {
                void ParameterDefaultValueHandler(string _, ParameterArgs args)
                {
                    args.Result = 0f;
                }

                bool CheckTagsPass(PlacementConfig placementConfig, Expression interpreter)
                {
                    if (placementConfig.Specifics == null || placementConfig.Specifics.IsEmpty())
                    {
                        Assert.IsFalse(placementConfig.IsStrictSpecifics);
                        return false;
                    }

                    var specificsDictionary = placementConfig.Specifics.AsNCalcDictionary();
                    Assert.IsNotNull(specificsDictionary);
                    interpreter.Parameters = specificsDictionary;
                    return (bool)interpreter.Evaluate();
                }

                var expression = new Expression(specificationExpression);
                expression.EvaluateParameter += ParameterDefaultValueHandler;
                var specifics = placementConfigs.Where(x => CheckTagsPass(x, expression)).ToArray();
                if (specifics.Any())
                    return _rnd.FromEnumerable(specifics);
                else
                    return null;
            }

            return _rnd.FromEnumerable(placementConfigs.Where(x => x.IsStrictSpecifics == false || x.Specifics.IsEmpty()));
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
