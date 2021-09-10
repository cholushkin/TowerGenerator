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
            public string ImportPath;
            public string OutputPath;
        }

        public Source[] Sources;
    }
}