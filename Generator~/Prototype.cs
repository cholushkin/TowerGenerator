using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerGenerator
{


    public class Prototype : MonoBehaviour
    {
        //public int SeedTopology;
        //public int SeedVisual;
        //public int SeedContent;

        public MetaProviderManager MetaProviderManager;
        public GeneratorNodes GeneratorNodes;

        void Reset()
        {
            MetaProviderManager = GetComponentInChildren<MetaProviderManager>();
            GeneratorNodes = GetComponentInChildren<GeneratorNodes>();

            //SeedTopology = -1;
            //SeedVisual = -1;
            //SeedContent = -1;
        }

        public void Init( long seed )
        {
            MetaProviderManager.Init();
            // set seed to GeneratorNodes and to configs
            GeneratorNodes.Init( seed, this );
        }
    }
}