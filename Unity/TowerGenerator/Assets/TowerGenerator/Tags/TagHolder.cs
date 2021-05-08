using UnityEngine;

namespace TowerGenerator
{
    public class TagHolder : MonoBehaviour
    {
        public TagSet TagSet;
    }

    public static class TagHelper
    {
        public static bool HasTag(this GameObject gObject, string tag, float threshold = 0.0f)
        {
            if (gObject == null)
                return false;
            var tagHolder = gObject.GetComponent<TagHolder>();
            if (tagHolder == null)
                return false;
            return tagHolder.HasTag(tag, threshold);
        }

        public static bool HasTag(this TagHolder tagHolder, string tag, float threshold = 0.0f)
        {
            if (tagHolder == null)
                return false;
            return tagHolder.TagSet.HasTag(tag, threshold);
        }
    }
}
