using UnityEngine;

namespace TowerGenerator
{
    public class CameraEntShowroomController : MonoBehaviour
    {
        public OrtoCameraSizeFitter CamSizeFitter1;
        public OrtoCameraSizeFitter CamSizeFitter2;

        public void FitView(GameObject current)
        {
            CamSizeFitter1.DoFit(current);
            CamSizeFitter2.DoFit(current);
        }
    }
}