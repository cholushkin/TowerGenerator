using System;
using UnityEngine;


namespace TowerGenerator
{
    [CreateAssetMenu(fileName = "Settings",
        menuName = "ScriptableObjects/TowerGeneratorSettings", order = 1)]
                                          
    public class TowerGeneratorSettings : ScriptableObject
    {
        [Serializable]
        public class Source
        {
            public string Name; // name of the source
            
            public bool EnableImport; // enable/disable importing for this source
            public bool EnableCleanupFbx; // enable/disable fbx cleanup

            public string ImportPath;
            public string OutputPath;
        }

        public Source[] Sources;
    }
}