using Assets.Plugins.Alg;
using UnityEngine;

namespace TowerGenerator
{

    public class SuppressedBy : BaseComponent
    {
        public string[] SuppressionLabels;

        public override bool IsValid()
        {
            if (SuppressionLabels == null)
                return false;

            if (SuppressionLabels.Length < 1)
                return false;

            if (transform.parent.GetComponent<Group>())
            {
                Debug.LogError(
                    $"SuppressedBy component is attached to the item controlled by a Group. Use nested (proxy)object instead of direct attach to this item: {transform.GetDebugName()}");
                return false;
            }

            foreach (var suppressionLabel in SuppressionLabels)
                if (!ChunkController.HasSuppressionLabel(suppressionLabel))
                    return false;

            return true;
        }
    }
}