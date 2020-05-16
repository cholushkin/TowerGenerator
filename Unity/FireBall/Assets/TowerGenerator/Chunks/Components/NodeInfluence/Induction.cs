using Events;

namespace TowerGenerator
{
    public class Induction : BaseComponent, IHandle<RootGroupsController.EventGroupChoiceDone>
    {
        public string[] InductionLabels;

        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;
#if DEBUG
            // todo: no node with InducedBy with such labels
#endif
            if (InductionLabels == null)
                return false;
            if (InductionLabels.Length < 1)
                return false;
            return true;
        }

        public void Handle(RootGroupsController.EventGroupChoiceDone message)
        {
            if (message.GroupChoice == OwnerGroup && gameObject.activeInHierarchy)
            {
                foreach (var suppressionLabel in InductionLabels)
                    GroupsController.Induce(suppressionLabel);
            }
        }
    }
}