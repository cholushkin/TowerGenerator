using System.Collections;
using TowerGenerator;
using UnityEngine;


namespace TowerGenerator
{
    public class ProgressPointerMoverFollowGenPointer : MonoBehaviour
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
                if (TopGens.Pointers.DistanceYFactorProgress2Generator() > 100)
                    TopGens.Pointers.MoveProgress();
            }
        }
    }
}