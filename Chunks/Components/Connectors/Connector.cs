using System;
using UnityEngine;


namespace TowerGenerator
{
    public class Connector : BaseComponent
    {
        [Flags]
        public enum ConnectorType
        {
            Undefined = 0,
            Pin = 1,
            Hole = 2
        }

        public Vector3 Normal;
        public Vector3 Forward;
        public float RotationSectorAngle;
        public ConnectorType ConnectorMode; // Pin, Hole or Pin+Hole
        public string[] ConnectExpressions;
    }

    public static class ConnectorHelper
    {

    }
}