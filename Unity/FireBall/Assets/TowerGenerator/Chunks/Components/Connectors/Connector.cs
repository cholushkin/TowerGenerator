using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TowerGenerator
{
    public class Connector : BaseComponent
    {
        public void SetNormal(Vector3 direction)
        {
            transform.forward = direction;
        }

        public Vector3 GetNormal()
        {
            return transform.forward;
        }
    }
}