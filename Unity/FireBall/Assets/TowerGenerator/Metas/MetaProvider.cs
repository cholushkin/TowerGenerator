using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Alg;
using Assets.Plugins.Alg;
using FireBall.Game;
using GameLib.Random;
using NCalc;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;


namespace TowerGenerator
{
    // responsibilies:
    // * providing metas by name
    // * providing metas by filter expression

    public class MetaProvider : Singleton<MetaProvider>
    {
        [Serializable]
        public class Filter
        {
            public Type EntityType;
            public Range Generation;
            public string Wildcard;

            public void SetTagExpressionToPass(string expression)
            {
                _expressionPass = new Expression(expression);
                //_expressionPass.EvaluateParameter += _parameterDefaultValueHandler;
                //_expressionPass.EvaluateFunction += _tagFunctionsHandler;
            }

            public Expression GetPassExpression()
            {
                return _expressionPass;
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }

            private Expression _expressionPass;
        }

        public MetaBase[] Metas;

        protected override void Awake()
        {
            base.Awake();
            var metas = Resources.LoadAll("Ents", typeof(MetaBase));
            Debug.Log($"Metas loaded: {metas.Length}");
        }


        /*
                public IEnumerable<MetaBase> GetMetas(Filter filter)
                {
                    Assert.IsNotNull(Metas);

                    //// no filter? just return original sequence
                    //if (filter == null)
                    //    return Metas.PatternMetas;

                    //IEnumerable<Metas.PatternMeta> filteredResult = Metas.PatternMetas;

                    //// generation filtering
                    //if (filter.Generation != null && !filter.Generation.IsZero()
                    //) // note: special case: zero range is special case
                    //    filteredResult = filteredResult.Where(x => filter.Generation.IsIn(x.PatternInfo.Generation));

                    //// name wildcard filtering
                    //if (!string.IsNullOrEmpty(filter.Wildcard))
                    //{
                    //    var wildcard = _wildCardToRegular(filter.Wildcard);
                    //    filteredResult = filteredResult.Where(x => Regex.IsMatch(x.Name, wildcard));
                    //}

                    //// tag expression pass filtering
                    //if (filter.GetPassExpression() != null)
                    //{
                    //    var interpeter = filter.GetPassExpression();
                    //    filteredResult = filteredResult.Where(x => _checkTagsPass(x, interpeter));
                    //}

                    //return filteredResult;
                }

                private static bool _checkTagsPass(MetaBase meta, Expression interpreter)
                {
                    if (meta.PatternInfo.TagSet == null || meta.PatternInfo.TagSet.IsEmpty())
                        return false;

                    Dictionary<string, object> metaParameters = new Dictionary<string, object>();
                    foreach (var tag in meta.PatternInfo.TagSet.Tags)
                    {
                        float value = float.TryParse(tag.Value, out value) ? value : 0f;
                        metaParameters.Add(tag.Name, value);
                    }

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

                private static string _wildCardToRegular(string value)
                {
                    return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
                }

                [ContextMenu("DbgTestFilters")]
                void DbgTestFilters()
                {
                    // no filter
                    {
                        Filter f1 = new Filter();
                        var metas = GetMetas(f1);
                        Debug.LogFormat("NO FILTER. Filter is: {0}. Result is: {1}", f1, metas.Count());
                    }

                    // range filter
                    {
                        Filter f1 = new Filter();

                        // (-1,0] range
                        f1.Generation = new Range(-1, 0);
                        f1.Generation.ToType = Range.EdgeType.Included;

                        var metas = GetMetas(f1);
                        Debug.LogFormat("RANGE FILTER. Filter is: {0}. Result is: {1}", f1, metas.Count());
                    }

                    // name wildcard filter
                    {
                        Filter f1 = new Filter { Wildcard = "*123*" };
                        var metas = GetMetas(f1);
                        Debug.LogFormat("WILDCARD FILTER. Filter is: {0}. Result is: {1}", f1, metas.Count());
                    }
                }

                [ContextMenu("DbgTestExpressions")]
                void DbgTestExpressions()
                {
                    string[] expressions =
                    {
                        "Has(Crazy) and ( Difficulty > 0.4 )",
                        "Has(Crazy)",
                        "Difficulty!=1",
                        "Difficulty==1",
                    };
                    foreach (var xpression in expressions)
                    {
                        Filter filter = new Filter();
                        filter.SetTagExpressionToPass(xpression);
                        var metas = GetMetas(filter);
                        Debug.LogFormat("{0} ---> {1} patterns ({2})",
                            xpression,
                            metas.Count(),
                            string.Join(", ", metas.Select(x => x.EntName).ToArray()));
                    }
                }
                */
    }
}