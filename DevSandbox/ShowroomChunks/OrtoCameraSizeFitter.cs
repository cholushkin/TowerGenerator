using Assets.Plugins.Alg;
using DG.Tweening;
using UnityEngine;


namespace TowerGenerator
{
    public class OrtoCameraSizeFitter : MonoBehaviour
    {
        public Camera Camera;
        public float Duration;

        public void Reset()
        {
            Camera = GetComponent<Camera>();
            Duration = 1.0f;
        }

        public void DoFit(GameObject gameObj)
        {
            var bbs = gameObj.BoundBox().size;
            Camera.DOOrthoSize(bbs.y, Duration);
        }
    }
}