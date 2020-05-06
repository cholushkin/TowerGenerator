using Assets.Plugins.Alg;
using DG.Tweening;
using UnityEngine;

namespace TowerGenerator
{
    public class CameraEntShowroomController : MonoBehaviour
    {
        public OrtoCameraSizeFitter CamSizeFitter1;
        public OrtoCameraSizeFitter CamSizeFitter2;

        public void FitView(GameObject current)
        {
            var bbs = current.BoundBox().size;
            CamSizeFitter1.GetComponent<Camera>().DOOrthoSize(bbs.y, 1f);
            CamSizeFitter2.GetComponent<Camera>().DOOrthoSize(bbs.y, 1f);
        }
    }
}