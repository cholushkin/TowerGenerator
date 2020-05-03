using UnityEngine;


namespace TowerGenerator
{

    public class SuppressedBy : BaseComponent
    {
        public string[] SuppressionLabels;

        public override bool IsValid()
        {
            throw new System.NotImplementedException();
        }
    }
}