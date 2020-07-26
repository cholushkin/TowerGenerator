using System.Collections;
using System.Collections.Generic;
using GameLib.Random;
using UnityEngine;


namespace TowerGenerator
{
    public class ConnectorCylindrical : Connector
    {
        public Range Height;
        public Range Radius;
        public Range HorizontalFOV; // for example 90-120 degree angle


    

        public Vector3 GetForward()
        {
            return transform.forward;
        }
    }
}