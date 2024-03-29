﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GameLib.Alg;
using UnityEngine;
using UnityEngine.Assertions;
using Range = GameLib.Random.Range;

namespace TowerGenerator
{
    public class MetaProviderGeneric<TMeta> : MonoBehaviour where TMeta : MetaBase
    {
        [Serializable]
        public class Filter
        {
            public Range Generation;
            //public TopologyType TopologyType;
            public Range BreadthRange;
            public Range HeightRange;
            public string Wildcard;
            public string Expression;


            public IEnumerable<TMeta> FilterGeneration(IEnumerable<TMeta> metas)
            {
                if (Generation == null)
                    return metas;
                return metas.Where(m => Generation.IsIn(m.Generation));
            }

            //public IEnumerable<MetaBase> FilterEntType(IEnumerable<MetaBase> metas)
            //{
            //    return metas.Where(m => (TopologyType & m.TopologyType) != 0);
            //}

            public IEnumerable<TMeta> FilterNameWildcard(IEnumerable<TMeta> metas)
            {
                if (string.IsNullOrEmpty(Wildcard))
                    return metas;
                var wildcard = _wildCardToRegular(Wildcard);
                return metas.Where(m => Regex.IsMatch(m.ChunkName, wildcard));
            }

            public IEnumerable<TMeta> FilterTagExpression(IEnumerable<TMeta> metas)
            {
                if (string.IsNullOrEmpty(Expression))
                    return metas;

                throw new NotImplementedException("xLua implementation");

                // todo: cache expression
                //var expression = new Expression(Expression);
                //expression.EvaluateParameter += _parameterDefaultValueHandler;
                //expression.EvaluateFunction += _tagFunctionsHandler;
                //return metas.Where(x => _checkTagsPass(x, expression));
            }

            public IEnumerable<TMeta> FilterSize(IEnumerable<TMeta> metas)
            {
                var breadth = BreadthRange;
                if (breadth == null || breadth.IsZero())
                    breadth = Range.InfiniteRange;
                var height = HeightRange;
                if (height == null || height.IsZero())
                    height = Range.InfiniteRange;

                if (breadth == Range.InfiniteRange && height == Range.InfiniteRange)
                    return metas;

                return metas;
                //return metas.Where(x=>_checkSizes(x, breadth, height));

            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }

            private static string _wildCardToRegular(string value)
            {
                return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
            }

            //private static bool _checkTagsPass(MetaBase meta, Expression interpreter)
            //{
            //    if (meta.TagSet == null || meta.TagSet.IsEmpty())
            //        return false;

            //    var metaParameters = meta.TagSet.AsNCalcDictionary();
            //    Assert.IsNotNull(metaParameters);
            //    interpreter.Parameters = metaParameters;
            //    return (bool)interpreter.Evaluate();
            //}

            //static void _parameterDefaultValueHandler(string name, ParameterArgs args)
            //{
            //    args.Result = 0f;
            //}

            //static void _tagFunctionsHandler(string name, FunctionArgs args)
            //{
            //    if (name == "Has")
            //    {
            //        float val = (float)args.Parameters[0].Evaluate();
            //        args.Result = val > 0f;
            //    }
            //}

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

            //static bool _checkSizes(MetaBase meta, Range breadthRange, Range heightRange)
            //{
            //    Assert.IsTrue(meta.AABBs.Count > 0);
            //    foreach (var metaAABB in meta.AABBs)
            //    {
            //        var xIsOK = metaAABB.x >= breadthRange.From && metaAABB.x <= breadthRange.To;
            //        var zIsOK = metaAABB.z >= breadthRange.From && metaAABB.z <= breadthRange.To;
            //        var heightIsOK = metaAABB.y >= heightRange.From && metaAABB.y <= heightRange.To;
            //        if (xIsOK && zIsOK && heightIsOK)
            //            return true;
            //    }
            //    return false;
            //}
        }

        public List<MetaProviderPopulatorGeneric<TMeta>> MetaPopulators;
        public bool InitOnAwake;
        public List<TMeta> Metas; // use this meta pool
        private bool _isInited;


        void Awake()
        {
            if(InitOnAwake && !_isInited)
                Init();
        }

        public void Init()
        {
            Debug.Log($"{transform.GetDebugName()}: initializing");
            Assert.IsFalse(_isInited);
            foreach (var populator in MetaPopulators)
                populator.Populate(this);
            Debug.Log($"{transform.GetDebugName()}: {Metas.Count} metas in the pool.");
            _isInited = true;
        }

        public IEnumerable<TMeta> GetMetas(Filter filter = null)
        {
            return FilterMetas(Metas, filter);
        }

        public static IEnumerable<TMeta> FilterMetas(IEnumerable<TMeta> metas, Filter filter = null)
        {
            if (filter == null) // no filter? just return original sequence
                return metas;

            IEnumerable<TMeta> filteredResult = metas;

            //filteredResult = filter.FilterEntType(filteredResult);
            filteredResult = filter.FilterGeneration(filteredResult);
            filteredResult = filter.FilterNameWildcard(filteredResult);
            filteredResult = filter.FilterTagExpression(filteredResult);
            filteredResult = filter.FilterSize(filteredResult);

            return filteredResult;
        }

        public void Populate(List<TMeta> metas)
        {
            Metas.AddRange(metas);
            Metas = Metas.Distinct().ToList();
        }


#if UNITY_EDITOR

        void DbgPrintMetas(IEnumerable<TMeta> metas, int cnt = -1)
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
            Debug.Log($"NO FILTER: count = {metas.Count()} of {Metas.Count}");
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