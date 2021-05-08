using System;
using Assets.Plugins.Alg;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class ChunkControllerDimensionsBased : ChunkControllerBase
    {
        public GroupStack DimensionStack;
        public int DimensionIndex = -1;

        public void SetDimensionIndex(int index = -1)
        {
            Assert.IsNotNull(DimensionStack);
            DimensionIndex = index;
        }

        public override Bounds CalculateDimensionAABB()
        {
            Assert.IsNotNull(DimensionStack);
            transform.ForEachChildrenRecursive(t => t.gameObject.SetActive(t.GetComponent<DimensionsIgnorant>() == null));
            DimensionStack.DoChoice(DimensionIndex);
            return CalculateCurrentAABB();
        }

        public override void ProcessGroupSetConfiguration(Group group, IPseudoRandomNumberGenerator rnd)
        {
            Assert.IsNotNull(DimensionStack);
            if (group == DimensionStack && DimensionIndex != -1)
                group.DoChoice(DimensionIndex);
            else
                group.DoRndChoice(rnd);
        }

        // for ChunkControllerDimensionsBased we ignore all connectors from the mesh and just calculate them and add manually
        public override Connector[] GetConnectors()
        {
            Assert.IsNotNull(DimensionStack);
            var aabb = CalculateCurrentAABB(false);

            // todo: remove all connectors (including children)
            Assert.IsTrue(TopologyType != TopologyType.Undefined);

            if (TopologyType == TopologyType.ChunkPeak)
            {
                var bottomConnector = gameObject.AddComponent<Connector>();
                bottomConnector.SetNormal(Vector3.down);
                bottomConnector.transform.position = aabb.center - new Vector3(0, aabb.extents.y, 0);
                return new Connector[] { bottomConnector };
            }
            else if (TopologyType == TopologyType.ChunkStd)
            {
                var bottomConnector = gameObject.AddComponent<Connector>();
                bottomConnector.SetNormal(Vector3.down);
                bottomConnector.transform.position = aabb.center - new Vector3(0, aabb.extents.y, 0);

                var topConnector = gameObject.AddComponent<Connector>();
                topConnector.SetNormal(Vector3.up);
                topConnector.transform.position = aabb.center + new Vector3(0, aabb.extents.y, 0);

                var leftConnector = gameObject.AddComponent<Connector>();
                leftConnector.SetNormal(Vector3.left);
                leftConnector.transform.position = aabb.center + new Vector3(-aabb.extents.x, 0, 0);

                var rightConnector = gameObject.AddComponent<Connector>();
                rightConnector.SetNormal(Vector3.right);
                rightConnector.transform.position = aabb.center + new Vector3(aabb.extents.x, 0, 0);

                var forwardConnector = gameObject.AddComponent<Connector>();
                forwardConnector.SetNormal(Vector3.forward);
                forwardConnector.transform.position = aabb.center + new Vector3(0, 0, aabb.extents.z);

                var backwardConnector = gameObject.AddComponent<Connector>();
                backwardConnector.SetNormal(Vector3.forward);
                backwardConnector.transform.position = aabb.center + new Vector3(0, 0, -aabb.extents.z);

                return new Connector[] { bottomConnector, topConnector, leftConnector, rightConnector, forwardConnector, backwardConnector };
            }
            else if (TopologyType == TopologyType.ChunkFoundation)
            {
                var topConnector = gameObject.AddComponent<Connector>();
                topConnector.SetNormal(Vector3.up);
                topConnector.transform.position = aabb.center + new Vector3(0, aabb.extents.y, 0);
            }
            else if (TopologyType == TopologyType.ChunkSideEar)
            {
                throw new NotImplementedException();
            }
            else if (TopologyType == TopologyType.ChunkBottomEar)
            {
                throw new NotImplementedException();
            }
            else if (TopologyType == TopologyType.ChunkTopEar)
            {
                throw new NotImplementedException();
            }
            else if (TopologyType == TopologyType.ChunkConnectorVertical)
            {
                throw new NotImplementedException();
            }
            else if (TopologyType == TopologyType.ChunkConnectorHorizontal)
            {
                throw new NotImplementedException();
            }

            return null;
        }
    }
}