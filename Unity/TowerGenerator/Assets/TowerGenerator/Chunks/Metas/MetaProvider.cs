using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Plugins.Alg;
using NCalc;
using UnityEngine;
using UnityEngine.Assertions;
using Range = GameLib.Random.Range;

namespace TowerGenerator
{
    public class MetaProvider : MonoBehaviour
    {
        [Serializable]
        public class Filter
        {
            public Range Generation;
            public TopologyType TopologyType;
            public Range BreadthRange;
            public Range HeightRange;
            public string Wildcard;
            public string Expression;


            public IEnumerable<MetaBase> FilterGeneration(IEnumerable<MetaBase> metas)
            {
                if (Generation == null)
                    return metas;
                return metas.Where(m => Generation.IsIn(m.Generation));
            }

            public IEnumerable<MetaBase> FilterEntType(IEnumerable<MetaBase> metas)
            {
                return metas.Where(m => (TopologyType & m.TopologyType) != 0);
            }

            public IEnumerable<MetaBase> FilterNameWildcard(IEnumerable<MetaBase> metas)
            {
                if (string.IsNullOrEmpty(Wildcard))
                    return metas;
                var wildcard = _wildCardToRegular(Wildcard);
                return metas.Where(m => Regex.IsMatch(m.ChunkName, wildcard));
            }

            public IEnumerable<MetaBase> FilterTagExpression(IEnumerable<MetaBase> metas)
            {
                if (string.IsNullOrEmpty(Expression))
                    return metas;                         

                // todo: cache expression
                var expression = new Expression(Expression);
                expression.EvaluateParameter += _parameterDefaultValueHandler;
                expression.EvaluateFunction += _tagFunctionsHandler;
                return metas.Where(x => _checkTagsPass(x, expression));
            }

            public IEnumerable<MetaBase> FilterSize(IEnumerable<MetaBase> metas)
            {
                var breadth = BreadthRange;
                if (breadth == null || breadth.IsZero())
                    breadth = Range.InfiniteRange;
                var height = HeightRange;
                if (height == null || height.IsZero())
                    height = Range.InfiniteRange;

                if (breadth == Range.InfiniteRange && height == Range.InfiniteRange)
                    return metas;

                return metas.Where(x=>_checkSizes(x, breadth, height));
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

            public static bool IsAABBInside(Vector3 AABB, Range breadthRange, Range heightRange)
            {
                if (breadthRange == null || breadthRange.IsZero())
                    breadthRange = Range.InfiniteRange;
                if (heightRange == null || heightRange.IsZero())
                    heightRange = Range.InfiniteRange;

                var xIsOK = AABB.x >= breadthRange.From && AABB.x <= breadthRange.To;
                var zIsOK = AABB.z >= breadthRange.From && AABB.z <= breadthRange.To;
                var heightIsOK = AABB.y >= heightRange.From && AABB.y <= heightRange.To;
                if (xIsOK && zIsOK && heightIsOK)
                    return true;

                return false;
            }

            static bool _checkSizes(MetaBase meta, Range breadthRange, Range heightRange)
            {
                Assert.IsTrue(meta.AABBs.Count > 0);
                foreach (var metaAABB in meta.AABBs)
                {
                    var xIsOK = metaAABB.x >= breadthRange.From && metaAABB.x <= breadthRange.To;
                    var zIsOK = metaAABB.z >= breadthRange.From && metaAABB.z <= breadthRange.To;
                    var heightIsOK = metaAABB.y >= heightRange.From && metaAABB.y <= heightRange.To;
                    if (xIsOK && zIsOK && heightIsOK)
                        return true;
                }
                return false;
            }
        }

        public MetaBase[] Metas;
        private bool _isInited;

        public void Init()
        {
            Assert.IsFalse(_isInited);
            if (Metas.Length != 0)
            {
                Debug.Log("Using defined metas set instead of loading");
                Debug.Log($"Metas using: {Metas.Length}");
            }
            else
            {
                Metas = Resources.LoadAll<MetaBase>(TowerGeneratorConstants.Chunks);
                Debug.Log($"{transform.GetDebugName()}: {Metas.Length} metas loaded.");
            }
            _isInited = true;
        }


        public IEnumerable<MetaBase> GetMetas(Filter filter = null)
        {
            return GetMetas(Metas, filter);
        }

        public static IEnumerable<MetaBase> GetMetas(IEnumerable<MetaBase> metas, Filter filter = null)
        {
            if (filter == null) // no filter? just return original sequence
                return metas;

            IEnumerable<MetaBase> filteredResult = metas;

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
            Debug.Log($"NO FILTER: count = {metas.Count()} of {Metas.Length}");
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