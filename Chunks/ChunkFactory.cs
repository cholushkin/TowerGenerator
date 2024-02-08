using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    public class ChunkFactory 
    {
        public enum Positioning
        {
            CenterOfAABB,
            ChunkPivot
        }
        private static IPseudoRandomNumberGenerator _rnd = RandomHelper.CreateRandomNumberGenerator();
        private static readonly float[] _angles = { 0f, 90f, 180f, 270f };

        public static Vector3 GetAttachPosition(Bounds parent, Vector3 attachDirection)
        {
            return Vector3.zero;
        }


        public static GameObject CreateChunkRnd(MetaBase meta, long seed, Transform parent, Vector3 position, Positioning positioning = Positioning.CenterOfAABB)
        {
            var pathInResources = ChunkImportSourceHelper.GetPathInResources(meta.ImportSource.ChunksOutputPath);
            var chunkPrefab = (GameObject)Resources.Load(pathInResources + "/" + meta.ChunkName);
            chunkPrefab.SetActive(false);
            var chunk = Object.Instantiate(chunkPrefab);
            
            chunk.name = chunkPrefab.name;
            chunk.transform.position = position;
            chunk.transform.SetParent(parent);
            chunk.transform.Rotate(chunk.transform.up, _rnd.FromArray(_angles));

            var baseChunkController = chunk.GetComponent<ChunkControllerBase>();
            baseChunkController.Seed = seed;
            baseChunkController.Init();
            chunk.SetActive(true);
            baseChunkController.SetConfiguration();

            // centering
            if (positioning == Positioning.CenterOfAABB)
            {
                var segBB = baseChunkController.CalculateCurrentAABB();
                var offset = baseChunkController.transform.position - segBB.center;
                chunk.transform.position += offset;
            }

            return chunk;
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