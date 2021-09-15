using System.Collections.Generic;
using System;
using System.Linq;
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

            public string Name;
            [Range(0, 1)]
            public float Value;
        }

        public List<Tag> Tags;


        public TagSet()
        {
            Tags = new List<Tag>();
        }

        public TagSet(TagSet otherTagset)
        {
            if (otherTagset.Tags != null)
            {
                Tags = new List<Tag>(otherTagset.Tags.Count);
                for (int i = 0; i < otherTagset.Tags.Count; ++i)
                    Tags.Add(new Tag(otherTagset.Tags[i].Name, otherTagset.Tags[i].Value));
            }
        }

        public void SetTag(Tag tag)
        {
            Assert.IsNotNull(tag);
            SetTag(tag.Name, tag.Value);
        }

        public void SetTag(string tagName, float value)
        {
            Tag existing = Tags.FirstOrDefault(x => x.Name == tagName);

            if (existing != null)
            {
                existing.Value = value;
            }
            else
            {
                Tags.Add(new Tag(tagName, value));
                UpdateFastAccessDictionaries();
            }
        }

        public bool HasTag(string tag, float threshold = 0.0f)
        {
            if (_dictionary.TryGetValue(tag, out var existed))
            {
                if (Math.Abs(existed.Value) < 0.0001f)
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
                result += $"[{tag.Name}:{tag.Value}]";
            return result;
        }

        private Dictionary<string, object> _nCalcDictionary;
        private Dictionary<string, Tag> _dictionary;

        void UpdateFastAccessDictionaries()
        {
            UpdateNCalcDictionary();
        }

        public void UpdateNCalcDictionary()
        {
            _nCalcDictionary = new Dictionary<string, object>();
            foreach (var tag in Tags)
                _nCalcDictionary.Add(tag.Name, tag.Value);
        }

        public Dictionary<string, object> AsNCalcDictionary()
        {
            if (_nCalcDictionary == null)
                UpdateNCalcDictionary();
            return _nCalcDictionary;
        }

        public void UpdateDictionary()
        {
            _dictionary = new Dictionary<string, Tag>();
            foreach (var tag in Tags)
                _dictionary.Add(tag.Name, new Tag(tag.Name, tag.Value));
        }

        public Dictionary<string, Tag> AsDictionary()
        {
            if (_dictionary == null)
                UpdateDictionary();
            return _dictionary;
        }
    }
}