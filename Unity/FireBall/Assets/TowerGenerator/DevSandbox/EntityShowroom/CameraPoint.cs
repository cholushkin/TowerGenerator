using TowerGenerator;
using UnityEngine;

namespace TowerGenerator
{
    public class CameraPoint : MonoBehaviour
    {
        public TopologyGeneratorsManifold Manifold;

        void Update()
        {
            var seg = Manifold.Pointers.PointerProgress;
            if (seg?.Data?.Topology == null)
                return;
            transform.position = seg.Data.Topology.Geometry.Bounds.center;
        }
    }
}