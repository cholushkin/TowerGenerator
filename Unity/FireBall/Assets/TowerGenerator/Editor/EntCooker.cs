using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class EntCooker
{
    public static GameObject Cook(GameObject semifinishedEnt)
    {
        Debug.Log("Cooking ent");

        BuildGroupsController(semifinishedEnt); // tree

        //var pat = designPattern.GetComponent<Pattern>();
        //ReorganizeBlocksHierarchy(pat);
        ////ConfigureConnections(pat);
        //SetLayer(pat.gameObject, GameConstants.LayerLevel);
        //ConfigureBG(pat);

        return semifinishedEnt;
    }

    private static void BuildGroupsController(GameObject ent)
    {
        var groupController = ent.AddComponent<GroupsController>();
        groupController.BuildImpactTree();
        groupController.CalculateBBMax();
        groupController.CalculateBBMin();
    }

    public static MetaBase CreateMeta(Entity ent, string dir, string name)
    {
        if (ent is ChunkStd)
        {
            var asset = ScriptableObject.CreateInstance<MetaChunkStd>();
            string assetPathAndName = dir + "/" + name + ".asset";
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            return asset;
        }

        return null;
    }
}
