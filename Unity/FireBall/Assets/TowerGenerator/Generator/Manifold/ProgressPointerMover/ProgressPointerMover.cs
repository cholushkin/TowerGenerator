using System.Collections;
using UnityEngine;

namespace TowerGenerator
{
    public class ProgressPointerMover : MonoBehaviour
    {
        public TopologyGeneratorsManifold TopGens;
        public float StepDelay;


        void Awake()
        {
            StartCoroutine(ProcessPointer());
        }

        IEnumerator ProcessPointer()
        {
            while (true)
            {
                yield return new WaitForSeconds(StepDelay);
                //TopGens.Pointers.MoveProgress();
            }
        }
    }
}

