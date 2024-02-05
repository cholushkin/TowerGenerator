using UnityEditor;

namespace TowerGenerator.Editor
{
    // Example of register a custom meta
    // Register MetaBase, which is default for a chunk
    [InitializeOnLoad]
    public static class RegisterMetaBaseCreator
    {
        static RegisterMetaBaseCreator()
        {
            MetaTypeRegistry.RegisterMetaType(new MetaTypeRegistry.MetaTypeEntry {Name = "MetaBase", Creator = new MetaBaseCreator()});
        }
    }
}