using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerGenerator
{
    public class MetaProviderPopulatorFromResourcesGeneric<TMeta> : MetaProviderPopulatorGeneric<TMeta> where TMeta : MetaBase
    {
        [Tooltip("Path inside Resources. If empty then all resources from entire project will be found")]
        public string ResourcesPath;
        
        public override  List<TMeta> FindMetas() => Resources.LoadAll<TMeta>(ResourcesPath).ToList();
    }
}