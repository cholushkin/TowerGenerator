using System;
using System.Collections;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class ChunkVisualConstructor : MonoBehaviour
    {
        private Blueprint _blueprint;
        private GeneratorPointer _pointers;
        private RandomHelper _rnd;
        private Transform _towerRoot;



        public void Init(Blueprint blueprint, Transform towerRoot, GeneratorPointer pointers)
        {
            Assert.IsNotNull(blueprint);
            Assert.IsNotNull(pointers);
            _pointers = pointers;
            _blueprint = blueprint;
            _towerRoot = towerRoot;
            StartCoroutine(ConstructionCycle());
        }

        IEnumerator ConstructionCycle()
        {
            while (true)
            {
                // moving up to the tree
                int counter = 0;
                var pointer = _pointers.PointerConstructFrom;
                if (pointer == null)
                {
                    yield return null;
                    continue;
                }

                while (true)
                {
                    if (!HasVisual(pointer)) // found?
                    {
                        foreach (var node in pointer.TraverseDepthFirstPreOrder()) 
                        {
                            Assert.IsTrue(node.Data.Visual != null && node.Data.Visual.ChunkTransform == null);
                            CreateVisualSegment(node);
                            if (node == _pointers.PointerConstructTo)
                                break;
                        }
                        break;
                    }
                    counter++;
                    if (pointer == _pointers.PointerConstructTo)
                        break;
                    pointer = pointer.Children[0];
                } 
                yield return null;
            }
        }

        private void CreateVisualSegment(TreeNode<Blueprint.Segment> node)
        {
            Assert.IsNotNull(node.Data.Topology);
            Assert.IsNull(node.Data.Visual.ChunkTransform);

            var chunk = ChunkFactory.CreateChunk( node.Data, _towerRoot );
            node.Data.Visual.ChunkTransform = chunk.transform;
        }

        private bool HasVisual(TreeNode<Blueprint.Segment> pointer)
        {
            return pointer.Data.Visual.ChunkTransform != null;
        }
    }
}
