using DG.Tweening;
using UnityEngine;

namespace TowerGenerator
{
    public class EntityPlace : MonoBehaviour
    {
        private GameObject _current;

        public void Place(MetaBase metaToPlace, long seed)
        {
            // remove _current
            Destroy(_current);

            // spawn new
            _current = EntityConstructor.ConstructEntity(metaToPlace, seed, transform);
            _current.transform.localScale = Vector3.zero;
            _current.transform.DOScale(100f, 1f).SetEase(Ease.OutElastic);
        }
    }


}
