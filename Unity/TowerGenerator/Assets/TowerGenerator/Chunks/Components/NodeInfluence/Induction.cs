using Events;

namespace TowerGenerator
{
    public class Induction : BaseComponent, IHandle<ChunkControllerBase.EventGroupChoiceDone>
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

        public void Handle(ChunkControllerBase.EventGroupChoiceDone message)
        {
            if (message.GroupChoice == InfluenceGroup && gameObject.activeInHierarchy)
            {
                foreach (var suppressionLabel in InductionLabels)
                    ChunkController.Induce(suppressionLabel);
            }
        }
    }
}