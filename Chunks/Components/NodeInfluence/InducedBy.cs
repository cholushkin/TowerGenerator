using Assets.Plugins.Alg;
using UnityEngine;

namespace TowerGenerator
{

    public class InducedBy : BaseComponent
    {
        public string[] InductionLabels;

        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;

            if (InductionLabels == null)
                return false;

            if (InductionLabels.Length < 1)
                return false;

            if (transform.parent.GetComponent<Group>())
            {
                Debug.LogError(
                    $"InducedBy component is attached to the item controlled by a Group. Use nested (proxy)object instead of direct attach to this item: {transform.GetDebugName()}");
                return false;
            }

            foreach (var inductionLabel in InductionLabels)
                if (!ChunkController.HasInductionLabel(inductionLabel))
                    return false;

            return true;
        }
    }
}