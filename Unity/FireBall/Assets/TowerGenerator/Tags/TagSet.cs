using System.Collections.Generic;
using System;
using UnityEngine;

namespace TowerGenerator
{
    [Serializable]
    public class TagSet
    {
        [Serializable]
        public class Tag
        {
            public string Name;
            public string Value;
        }
        public List<Tag> Tags;
        private Dictionary<string, object> _tagsDictionary;

        public bool IsEmpty()
        {
            return (Tags == null || Tags.Count == 0);
        }

        public Dictionary<string, object> AsDictionary()
        {
            if (IsEmpty())
                return null;
            
            // lazy resource initialization
            if (_tagsDictionary != null)
                return _tagsDictionary;

            _tagsDictionary = new Dictionary<string, object>();
            foreach (var tag in Tags)
            {
                float value = 0f;
                var isOK = float.TryParse(tag.Value, out value);
                if(!isOK)
                    Debug.LogError($"Error while parsing tag value: tag name: {tag.Name}");
                _tagsDictionary.Add(tag.Name, value);
            }
            return _tagsDictionary;
        }

        public override string ToString()
        {
            if (Tags == null || Tags.Count == 0)
            {
                return "no tags";
            }
            var result = "";
            foreach (var tag in Tags)
                result += string.IsNullOrEmpty(tag.Value) ? $"[{tag.Name}]" : $"[{tag.Name}:{tag.Value}]";
            return result;
        }
    }
}
