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
            In = 1,
            Out = 2
        }

        public Vector3 Normal;
        public Vector3 Forward;
        public float RotationSectorAngle;
        public ConnectorType ConnectorMode; // In, Out or InOut
        public string[] ConnectExpressions;
    }

    public static class ConnectorHelper
    {

    }
}