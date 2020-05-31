using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Alg;
using Assets.Plugins.Alg;
using GameLib.Random;
using NCalc;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class MetaProvider : Singleton<MetaProvider>
    {
        [Serializable]
        public class Filter
        {
            private readonly Range _generation;
            private readonly TopologyType _topologyType;
            private readonly Range _breadthRange;
            private readonly Range _heightRange;
            private readonly string _wildcard;
            private Expression _expression;

            private string ExpressionPass
            {
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        _expression = null;
                        return;
                    }
                    _expression = new Expression(value);
                    _expression.EvaluateParameter += _parameterDefaultValueHandler;
                    _expression.EvaluateFunction += _tagFunctionsHandler;
                }
            }

            public Filter( Range generationRange = null,
                TopologyType topology = (TopologyType) 0b1111111111111111,
                string wildCard = null, string tagExpression = null, Range breadthRange = null,
                Range heightRange = null )
            {
                _generation = generationRange;
                _topologyType = topology;
                _wildcard = wildCard;
                _breadthRange = breadthRange;
                _heightRange = heightRange;
                ExpressionPass = tagExpression;
            }

            public IEnumerable<MetaBase> FilterGeneration(IEnumerable<MetaBase> metas)
            {
                if (_generation == null)
                    return metas;
                return metas.Where(m => _generation.IsIn(m.Generation));
            }

            public IEnumerable<MetaBase> FilterEntType(IEnumerable<MetaBase> metas)
            {
                return metas.Where(m => (_topologyType & m.TopologyType) != 0);
            }

            public IEnumerable<MetaBase> FilterNameWildcard(IEnumerable<MetaBase> metas)
            {
                if (string.IsNullOrEmpty(_wildcard))
                    return metas;
                var wildcard = _wildCardToRegular(_wildcard);
                return metas.Where(m => Regex.IsMatch(m.ChunkName, wildcard));
            }

            public IEnumerable<MetaBase> FilterTagExpression(IEnumerable<MetaBase> metas)
            {
                if (_expression == null)
                    return metas;
                return metas.Where(x => _checkTagsPass(x, _expression));
            }

            public IEnumerable<MetaBase> FilterSize(IEnumerable<MetaBase> metas)
            {
                if (_breadthRange == null || _heightRange == null)
                    return metas;
                return metas.Where(_checkSizes);
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }

            private static string _wildCardToRegular(string value)
            {
                return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
            }

            private static bool _checkTagsPass(MetaBase meta, Expression interpreter)
            {
                if (meta.TagSet == null || meta.TagSet.IsEmpty())
                    return false;

                var metaParameters = meta.TagSet.AsNCalcDictionary();
                Assert.IsNotNull(metaParameters);
                interpreter.Parameters = metaParameters;
                return (bool)interpreter.Evaluate();
            }

            static void _parameterDefaultValueHandler(string name, ParameterArgs args)
            {
                args.Result = 0f;
            }

            static void _tagFunctionsHandler(string name, FunctionArgs args)
            {
                if (name == "Has")
                {
                    float val = (float)args.Parameters[0].Evaluate();
                    args.Result = val > 0f;
                }
            }

            private bool _checkSizes(MetaBase meta)
            {
                Assert.IsTrue(meta.AABBs.Count > 0);
                foreach (var metaAABB in meta.AABBs)
                {
                    var breadthIsOK = metaAABB.x >= _breadthRange.From && metaAABB.z <= _breadthRange.To;
                    var heightIsOK = metaAABB.y >= _heightRange.From && metaAABB.y <= _heightRange.To;
                    if (breadthIsOK && heightIsOK)
                        return true;
                }

                return false;
            }
        }

        public MetaBase[] Metas;

        protected override void Awake()
        {
            base.Awake();
            if (Metas.Length != 0)
            {
                Debug.LogWarning("Using defined metas set instead of loading");
                Debug.Log($"Metas using: {Metas.Length}");
            }
            else
            {
                Metas = Resources.LoadAll<MetaBase>(TowerGeneratorConstants.Chunks);
                Debug.Log($"{transform.GetDebugName()}: {Metas.Length} metas loaded.");
            }
        }


        public IEnumerable<MetaBase> GetMetas(Filter filter = null)
        {
            Assert.IsNotNull(Metas);

            if (filter == null) // no filter? just return original sequence
                return Metas;

            IEnumerable<MetaBase> filteredResult = Metas;

            filteredResult = filter.FilterEntType(filteredResult);
            filteredResult = filter.FilterGeneration(filteredResult);
            filteredResult = filter.FilterNameWildcard(filteredResult);
            filteredResult = filter.FilterTagExpression(filteredResult);
            filteredResult = filter.FilterSize(filteredResult);

            return filteredResult;
        }


#if UNITY_EDITOR

        void DbgPrintMetas(IEnumerable<MetaBase> metas, int cnt = -1)
        {
            foreach (var metaBase in metas)
            {
                Debug.Log($"{metaBase}");
            }
        }

        [ContextMenu("DbgTestNoFilter")]
        void DbgTestNoFilter()
        {
            var metas = GetMetas();
            Debug.Log($"NO FILTER: count = {metas.Count()} of {MetaProvider.Instance.Metas.Length}");
            DbgPrintMetas(metas);
        }

        //// range filter
        //    {
        //        Filter f1 = new Filter();

        //        // (-1,0] range
        //        f1.Generation = new Range(-1, 0);
        //        f1.Generation.ToType = Range.EdgeType.Included;

        //        var metas = GetMetas(f1);
        //        Debug.LogFormat("RANGE FILTER. Filter is: {0}. Result is: {1}", f1, metas.Count());
        //    }

        //    // name wildcard filter
        //    {
        //        Filter f1 = new Filter { Wildcard = "*123*" };
        //        var metas = GetMetas(f1);
        //        Debug.LogFormat("WILDCARD FILTER. Filter is: {0}. Result is: {1}", f1, metas.Count());
        //    }
        //}

        //[ContextMenu("DbgTestExpressions")]
        //void DbgTestExpressions()
        //{
        //    string[] expressions =
        //    {
        //                "Has(Crazy) and ( Difficulty > 0.4 )",
        //                "Has(Crazy)",
        //                "Difficulty!=1",
        //                "Difficulty==1",
        //            };
        //    foreach (var xpression in expressions)
        //    {
        //        Filter filter = new Filter();
        //        filter.SetTagExpressionToPass(xpression);
        //        var metas = GetMetas(filter);
        //        Debug.LogFormat("{0} ---> {1} patterns ({2})",
        //            xpression,
        //            metas.Count(),
        //            string.Join(", ", metas.Select(x => x.EntName).ToArray()));
        //    }
        //}

#endif
    }
}