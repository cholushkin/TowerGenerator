
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

            foreach (var inductionLabel in InductionLabels)
                if (!GroupsController.HasInductionLabel(inductionLabel))
                    return false;

            return true;
        }
    }
}