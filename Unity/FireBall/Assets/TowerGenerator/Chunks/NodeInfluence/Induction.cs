using UnityEngine;

namespace TowerGenerator
{
    public class Induction : BaseComponent
    {
        public string[] InductionLabels;

        public override bool IsValid()
        {
            throw new System.NotImplementedException();
        }

        public void OnEnable()
        {
            foreach (var inductionLabel in InductionLabels)
                GroupsController.Induce(inductionLabel);
        }
    }
}