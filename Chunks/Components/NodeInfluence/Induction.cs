using Events;

namespace TowerGenerator
{
    public class Induction : BaseComponent
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
    }
}