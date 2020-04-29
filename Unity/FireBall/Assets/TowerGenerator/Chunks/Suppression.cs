using UnityEngine;


namespace TowerGenerator
{

    public class Suppression : MonoBehaviour, INodeValidation
    {
        public string[] SuppressionLabels;

        public bool IsValid()
        {
            // empty SuppressionLabels
            // no node with SuppressedBy with such labels
            return true;
        }

        public void Fix()
        {

        }
    }
}