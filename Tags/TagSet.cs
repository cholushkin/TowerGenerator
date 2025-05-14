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
                Assert.IsFalse(string.IsNullOrEmpty(name), "Tag name cannot be null or empty.");

                Name = name;
                Value = value;
            }

            public string Name;
            public float Value;
        }

        public List<Tag> Tags = new();
        private Dictionary<string, Tag> _tagDictionary;

        // Lazy initialization for _tagDictionary
        private void EnsureTagDictionaryInitialized()
        {
            if (_tagDictionary == null)
            {
                _tagDictionary = new Dictionary<string, Tag>();
                foreach (var tag in Tags)
                    _tagDictionary[tag.Name] = tag;
            }
        }

        public void Set(string tagName, float value = 1.0f)
        {
            EnsureTagDictionaryInitialized(); // Make sure dictionary is initialized
            
            // Try to get the existing tag by name from the dictionary
            if (_tagDictionary.TryGetValue(tagName, out var existingTag))
            {
                existingTag.Value = value; // Update the value if the tag exists
            }
            else
            {
                // If the tag doesn't exist, create a new one and add it to both the list and dictionary
                var newTag = new Tag(tagName, value);
                Tags.Add(newTag);
                _tagDictionary[tagName] = newTag;
            }
        }

        public float Get(string tagName, float defaultValue = 0f)
        {
            EnsureTagDictionaryInitialized(); // Make sure dictionary is initialized

            if (_tagDictionary.TryGetValue(tagName, out var tag))
                return tag.Value;
            return defaultValue;
        }

        public bool HasTag(string tag)
        {
            EnsureTagDictionaryInitialized(); // Make sure dictionary is initialized

            if (_tagDictionary.TryGetValue(tag, out var existingTag))
                return existingTag.Value >= 0f;
            return false;
        }

        public bool IsEmpty()
        {
            return Tags.Count == 0;
        }

        public override string ToString()
        {
            if (IsEmpty())
            {
                return "|no tags|";
            }

            return string.Join(", ", Tags.Select(tag => $"[{tag.Name}:{tag.Value}]"));
        }
    }
}
