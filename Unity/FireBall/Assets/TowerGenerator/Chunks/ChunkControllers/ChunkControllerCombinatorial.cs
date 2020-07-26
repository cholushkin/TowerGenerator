using System.Collections;
using System.Collections.Generic;
using Assets.Plugins.Alg;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class ChunkControllerCombinatorial : ChunkControllerBase
    {
        public override Connector[] GetConnectors()
        {
            var activeConnectors = GetComponentsInChildren<Connector>(false);
            Assert.IsTrue(activeConnectors.Length > 0);
            return activeConnectors;
        }
    }
}
