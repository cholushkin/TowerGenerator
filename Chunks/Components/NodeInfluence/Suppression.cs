

namespace TowerGenerator
{

    public class Suppression : BaseComponent
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
    }
}