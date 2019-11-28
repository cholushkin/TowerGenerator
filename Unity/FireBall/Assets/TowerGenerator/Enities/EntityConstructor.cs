using System.Collections;
using System.Collections.Generic;
using Alg;
using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{

    public class EntityConstructor 
    {
        private static RandomHelper _rnd = new RandomHelper(-1);
        private static readonly float[] _angles = { 0f, 90f, 180f, 270f };

        public static Vector3 GetAttachPosition(Bounds parent, Vector3 attachDirection)
        {
            return Vector3.zero;
        }

        public static GameObject ConstructEntity( MetaBase meta, long seed, Transform parent)
        {
            var visSegPrefab = (GameObject)Resources.Load("Ents/" + meta.EntName);
            var visSegment = Object.Instantiate(visSegPrefab);
            visSegment.name = visSegPrefab.name;

            visSegment.transform.position = parent.position;
            visSegment.transform.SetParent(parent);

            // rotation 
            visSegment.transform.Rotate(visSegment.transform.up, _rnd.FromArray(_angles));

            var visSegController = visSegment.GetComponent<GroupsController>();
            visSegController.Seed = seed;
            visSegController.SetRndConfiguration();

            // centering
            var segBB = visSegController.CalculateBB();
            var offset = visSegment.transform.position - segBB.center;
            visSegment.transform.position += offset;
            segBB.center = visSegment.transform.position;
            return visSegment;
        }
    }
}