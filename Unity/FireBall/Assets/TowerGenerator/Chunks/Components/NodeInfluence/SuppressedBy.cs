
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

            foreach (var suppressionLabel in SuppressionLabels)
                if (!GroupsController.HasSuppressionLabel(suppressionLabel))
                    return false;

            return true;
        }
    }
}