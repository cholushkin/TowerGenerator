using DG.Tweening;
using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    public class EntityPlace : MonoBehaviour
    {
        private GameObject _current;
        private ChunkControllerBase _chunkController;
        public CameraEntShowroomController CameraEntShowroomController;

        public void Place(MetaBase metaToPlace, IPseudoRandomNumberGeneratorState seed)
        {
            // remove _current
            Destroy(_current);

            // spawn new
            _current = ChunkFactory.CreateChunkRnd(metaToPlace, seed, transform, transform.position);
            CameraEntShowroomController.FitView(_current);
            //_current.transform.localScale = Vector3.zero;
            //_current.transform.DOScale(100f, 1f).SetEase(Ease.OutElastic);
            _chunkController = _current.GetComponent<ChunkControllerBase>();

        }

        void OnDrawGizmos()
        {
            if (_current != null)
            {
                //Gizmos.DrawLine(Vector3.zero, Vector3.zero+Vector3.right*100f);
                var bounds = _chunkController.CalculateCurrentAABB();
                Gizmos.DrawWireCube(
                    bounds.center,
                    bounds.size);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(bounds.center, 0.25f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.position, 0.25f);
            }
        }
    }
}
