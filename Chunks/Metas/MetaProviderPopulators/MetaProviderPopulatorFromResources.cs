using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerGenerator
{
    public class MetaProviderPopulatorFromResources : MetaProviderPopulatorBase
    {
        [Tooltip("Path inside Resources. If empty then all resources from entire project will be found")]
        public string ResourcesPath;
        
        public override  List<MetaBase> FindMetas() => Resources.LoadAll<MetaBase>(ResourcesPath).ToList();
    }
}