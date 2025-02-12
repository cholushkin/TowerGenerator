using UnityEngine;

namespace TowerGenerator
{
    public class TagHolder : MonoBehaviour
    {
        public TagSet TagSet;
    }

    public static class TagHelper
    {
        public static bool CheckTag(this GameObject gObject, string tag, float atLeast = 1.0f)
        {
            if (gObject == null)
                return false;
            var tagHolder = gObject.GetComponent<TagHolder>();
            if (tagHolder == null)
                return false;
            return tagHolder.CheckTag(tag, atLeast);
        }

        public static bool CheckTag(this TagHolder tagHolder, string tag, float atLeast = 1.0f)
        {
            if (tagHolder == null)
                return false;
            return tagHolder.TagSet.Get(tag, atLeast) >= atLeast;
        }
    }
}
