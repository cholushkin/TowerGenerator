using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TowerGenerator
{
    public class Connector : BaseComponent
    {
        public string[] SupportedTags;
        // todo: expressions with tags to calculate compatibility of chunks
        // todo: add FbxCommand FilterConnection to support expressions 
        public void SetNormal(Vector3 direction)
        {
            transform.forward = direction;
        }

        public Vector3 GetNormal()
        {
            return transform.forward;
        }

        public void SetForward(Vector3 direction)
        {
        }

        public Vector3 GetForward()
        {
            return transform.forward;
        }
    }
}