using System;
using UnityEditor;
using UnityEngine;


namespace TowerGenerator
{
    [FilePath("TowerGenerator/Settings/Settings.asset", FilePathAttribute.Location.ProjectFolder)]
    [CreateAssetMenu(fileName = "Settings",
        menuName = "ScriptableObjects/TowerGeneratorSettings", order = 1)]
                                          
    public class TowerGeneratorSettings : ScriptableSingleton<TowerGeneratorSettings>
    {
        [Serializable]
        public class Source
        {
            public string Name;
            public bool IsEnabled;
            public string ImportPath;
            public string OutputPath;
        }

        public Source[] Sources;
    }
}