using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    public class ChunkFactory 
    {
        private static RandomHelper _rnd = new RandomHelper(-1);
        private static readonly float[] _angles = { 0f, 90f, 180f, 270f };

        public static Vector3 GetAttachPosition(Bounds parent, Vector3 attachDirection)
        {
            return Vector3.zero;
        }

        public static GameObject CreateChunk( MetaBase meta, long seed, Transform parent)
        {
            var visSegPrefab = (GameObject)Resources.Load("Chunks/" + meta.ChunkName);
            visSegPrefab.SetActive(false);
            var visSegment = Object.Instantiate(visSegPrefab);
            visSegment.name = visSegPrefab.name;

            visSegment.transform.position = parent.position;
            visSegment.transform.SetParent(parent);

            // rotation 
            visSegment.transform.Rotate(visSegment.transform.up, _rnd.FromArray(_angles));

            var visSegController = visSegment.GetComponent<RootGroupsController>();
            visSegController.Seed = seed;
            visSegController.Init();
            visSegment.SetActive(true);
            visSegController.SetConfiguration();

            // centering
            var segBB = visSegController.CalculateBB();
            Debug.Log($"sebBB:{segBB}");
            var offset = visSegController.transform.position - segBB.center;
            Debug.Log($"offset:{offset}");
            visSegment.transform.position += offset;
            segBB.center = visSegment.transform.position;
            return visSegment;
        }
    }
}