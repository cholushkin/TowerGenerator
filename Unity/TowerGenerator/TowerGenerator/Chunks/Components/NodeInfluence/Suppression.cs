using Events;
using UnityEngine;


namespace TowerGenerator
{

    public class Suppression : BaseComponent, IHandle<ChunkControllerBase.EventGroupChoiceDone>
    {
        public string[] SuppressionLabels;

        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;
#if DEBUG
            // todo: no node with SuppressedBy with such labels
#endif
            if (SuppressionLabels == null)
                return false;
            if (SuppressionLabels.Length < 1)
                return false;
            return true;
        }
 
        public void Handle(ChunkControllerBase.EventGroupChoiceDone message)
        {
            if (message.GroupChoice == InfluenceGroup && gameObject.activeInHierarchy)
            {
                foreach (var suppressionLabel in SuppressionLabels)
                    ChunkController.Suppress(suppressionLabel);
            }
        }
    }
}