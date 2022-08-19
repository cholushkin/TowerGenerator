using UnityEngine;


namespace TowerGenerator
{
    [CreateAssetMenu(fileName = "ColorSchemeImportSource",
        menuName = "ScriptableObjects/ColorSchemeImportSource", order = 1)]


    public class ColorSchemeImportSource : ScriptableObject
    {
        public string ImportPath;
        public string OutputPath;
    }
}