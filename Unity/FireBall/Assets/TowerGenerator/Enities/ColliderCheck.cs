using System.Collections.Generic;
using System.Linq;
using Assets.Plugins.Alg;
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public class ColliderCheck : MonoBehaviour
    {
        public enum CollisionCheckType
        {
            MeshBased,
            AABBBased
        }

        public CollisionCheckType CollisionCheck;

        public virtual void Configure(Transform entityRoot, List<FbxProps.ScriptToAdd.ScriptProperty> scriptProperties)
        {

            PropertyParserHelper.CheckPropNames(scriptProperties, PropertyParserHelper.PropNameCollisionCheck);
            var propCollisionCheck = scriptProperties.FirstOrDefault(x => x.PropName == PropertyParserHelper.PropNameCollisionCheck);
            Assert.IsNotNull(propCollisionCheck);

            var lowered = propCollisionCheck.PropValue.ToLower();
            if (lowered == "meshbased")
                CollisionCheck = CollisionCheckType.MeshBased;
            else if (lowered == "aabbbased")
                CollisionCheck = CollisionCheckType.AABBBased;
            else
            {
                Debug.LogError($"Bad property value {gameObject.transform.GetDebugName()} prop: {propCollisionCheck}");
            }
        }
    }
}
