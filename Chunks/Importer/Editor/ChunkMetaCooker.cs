using TowerGenerator.Editor;
using UnityEngine;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkMetaCooker
    {
        public static MetaBase Cook(GameObject chunkObject, ChunkImportSource importSource, ChunkCooker.ChunkImportState importState)
        {
            if (string.IsNullOrEmpty(importState.MetaType))
            {
                Debug.LogError($"{importState.ChunkName} has no MetaType specified");
                return null;
            }

            var creator = MetaTypeRegistry.GetCreatorOfMetaType(importState.MetaType);
            if (creator == null)
            {
                Debug.LogError($"Can't find registered Meta Creator for '{importState.MetaType}'");
                return null;
            }

            var meta = creator.Create(chunkObject, importSource, importState);
            return meta;
        }
    }
}