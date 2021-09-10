using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    public class ChunkFactory 
    {
        private static IPseudoRandomNumberGenerator _rnd = RandomHelper.CreateRandomNumberGenerator();
        private static readonly float[] _angles = { 0f, 90f, 180f, 270f };

        public static Vector3 GetAttachPosition(Bounds parent, Vector3 attachDirection)
        {
            return Vector3.zero;
        }


        public static GameObject CreateChunkRnd(MetaBase meta, IPseudoRandomNumberGeneratorState seed, Transform parent, Vector3 position)
        {
            var visSegPrefab = (GameObject)Resources.Load("Chunks/" + meta.ChunkName);
            visSegPrefab.SetActive(false);
            var visSegment = Object.Instantiate(visSegPrefab);
            visSegment.name = visSegPrefab.name;

            visSegment.transform.position = position;
            visSegment.transform.SetParent(parent);

            // rotation 
            visSegment.transform.Rotate(visSegment.transform.up, _rnd.FromArray(_angles));

            var baseChunkController = visSegment.GetComponent<ChunkControllerBase>();
            baseChunkController.Seed = seed.AsNumber();
            baseChunkController.Init();
            visSegment.SetActive(true);
            baseChunkController.SetConfiguration();

            // centering
            var segBB = baseChunkController.CalculateCurrentAABB();
            var offset = baseChunkController.transform.position - segBB.center;
            visSegment.transform.position += offset;
            return visSegment;
        }

        public static GameObject CreateChunk( Blueprint.Segment bpSegment, Transform parent)
        {
            var topology = bpSegment.Topology;

            var visSegPrefab = (GameObject)Resources.Load("Chunks/" + topology.Geometry.Meta.ChunkName);
            visSegPrefab.SetActive(false);
            var visSegment = Object.Instantiate(visSegPrefab);
            visSegment.name = visSegPrefab.name;

            visSegment.transform.position = parent.position + topology.Geometry.Bounds.center;
            visSegment.transform.SetParent(parent);

            // rotation 
            visSegment.transform.Rotate(visSegment.transform.up, _rnd.FromArray(_angles));

            var visSegController = visSegment.GetComponent<ChunkControllerBase>();
            visSegController.Seed = bpSegment.Visual.Seed;
            visSegController.Init();
            visSegment.SetActive(true);
            visSegController.SetConfiguration();

            // centering
            var segBB = visSegController.CalculateCurrentAABB();
            var offset = visSegController.transform.position - segBB.center;
            visSegment.transform.position += offset;
            return visSegment;
        }
    }
}