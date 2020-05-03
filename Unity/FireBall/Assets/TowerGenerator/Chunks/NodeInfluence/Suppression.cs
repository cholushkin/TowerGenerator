using UnityEngine;


namespace TowerGenerator
{

    public class Suppression : BaseComponent
    {
        public string[] SuppressionLabels;

        public override bool IsValid()
        {
            // empty SuppressionLabels
            // no node with SuppressedBy with such labels
            return true;
        }
    }
}