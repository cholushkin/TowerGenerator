#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.Assertions;

namespace TowerGenerator.Editor
{
    [InitializeOnLoad]
    public static class MetaTypeRegistry
    {
        public class MetaTypeEntry
        {
            public string Name;
            public IMetaCreator Creator;
        }

        private static List<MetaTypeEntry> _metaTypesRegistered;

        public static List<MetaTypeEntry> GetRegistered() => _metaTypesRegistered;

        public static void RegisterMetaType(MetaTypeEntry entry)
        {
            if (_metaTypesRegistered == null)
                _metaTypesRegistered = new List<MetaTypeEntry>();

            Assert.IsFalse(_metaTypesRegistered.Contains(entry), $"already registered {entry.Name}");
            _metaTypesRegistered.Add(entry);
        }

        public static IMetaCreator GetCreatorOfMetaType(string metaTypeName)
        {
            var entry = _metaTypesRegistered.FirstOrDefault(x => x.Name == metaTypeName);
            return entry?.Creator;
        }
    }
}
#endif