using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    [Serializable]
    public class TagSet
    {
        [Serializable]
        public class Tag
        {
            public Tag(string name, float value)
            {
                Assert.IsTrue(value >= 0.0f);
                Assert.IsTrue(value <= 1.0f);
                Assert.IsTrue(!string.IsNullOrEmpty(name));

                value = Mathf.Clamp01(value);

                Name = name;
                Value = value;
            }

            public string Name { get; private set; }
            public float Value { get; set; }
        }

        public Dictionary<string, Tag> Tags;

        public void SetTag(Tag tag)
        {
            Assert.IsNotNull(tag);
            SetTag(tag.Name, tag.Value);
        }

        public void SetTag(string tagName, float value)
        {
            if (Tags == null)
                Tags = new Dictionary<string, Tag>();

            Tag existed;
            if (Tags.TryGetValue(tagName, out existed))
            {
                existed.Value = value;
            }
            else
            {
                Tags.Add(tagName, new Tag(tagName, value));
            }
        }

        public bool HasTag(string tag, float threshold = 0.0f)
        {
            if (Tags.TryGetValue(tag, out var existed))
            {
                if (existed.Value == 0.0f)
                    return false;
                if (existed.Value >= threshold)
                    return true;
            }
            return false;
        }

        public bool IsEmpty()
        {
            return (Tags == null || Tags.Count == 0);
        }

        public override string ToString()
        {
            if (IsEmpty())
            {
                return "|no tags|";
            }
            var result = "";
            foreach (var tag in Tags)
                result += $"[{tag.Value.Name}:{tag.Value.Value}]";
            return result;
        }


        private Dictionary<string, object> _nCalcDictionary;
        public Dictionary<string, object> AsNCalcDictionary()
        {
            if (IsEmpty())
                return null;

            // lazy resource initialization
            if (_nCalcDictionary != null)
                return _nCalcDictionary;

            _nCalcDictionary = new Dictionary<string, object>();
            foreach (var kv in Tags)
                _nCalcDictionary.Add(kv.Value.Name, kv.Value.Value);

            return _nCalcDictionary;
        }
    }
}
