using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerGenerator.ChunkImporter
{
    public class FbxProps : MonoBehaviour
    {
        [Serializable]
        public class Property
        {
            public string Name;
            public string Value;

            public Property(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        public List<Property> Properties;

        public void AddProperty(string name, string value)
        {
            if (Properties == null)
                Properties = new List<Property>();
            Properties.Add(new Property(name, value));
        }
    }
}

