using System.Linq;
using UnityEngine;

namespace TowerGenerator
{
    public class MetaProviderPopulatorFromResources : MetaProviderPopulatorBase
    {
        [Tooltip("Path inside Resources. If empty then all resources from entire project will be found")]
        public string ResourcesPath;
        
        public override void FindMetas()
        {
            var allMetas = Resources.LoadAll<MetaBase>(ResourcesPath);
            FoundMetas = allMetas.ToList();
        }
    }
}