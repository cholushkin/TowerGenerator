using Events;
using UnityEngine;


namespace TowerGenerator
{

    public class Suppression : BaseComponent, IHandle<RootGroupsController.EventGroupChoiceDone>
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
 
        public void Handle(RootGroupsController.EventGroupChoiceDone message)
        {
            if (message.GroupChoice == OwnerGroup && gameObject.activeInHierarchy)
            {
                foreach (var suppressionLabel in SuppressionLabels)
                    GroupsController.Suppress(suppressionLabel);
            }
        }
    }
}