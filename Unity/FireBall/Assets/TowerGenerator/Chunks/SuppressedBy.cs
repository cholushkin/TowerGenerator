using UnityEngine;


namespace TowerGenerator
{

    public class SuppressedBy : MonoBehaviour, INodeValidation
    {
        public string[] SuppressionLabels;

        public bool IsValid()
        {
            throw new System.NotImplementedException();
        }

        public void Fix()
        {
            throw new System.NotImplementedException();
        }
    }
}